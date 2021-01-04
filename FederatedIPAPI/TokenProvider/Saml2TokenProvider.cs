using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens.Saml2;
using FederatedIPAPI.Configuration;
using FederatedIPAPI.Models;
using Microsoft.Extensions.Options;

namespace FederatedIPAPI.TokenProvider
{
    public interface ITokenProvider {
        string Create(IEnumerable<TokenClaim> claims);
        string Renew(string tokenStr, IEnumerable<TokenClaim> claims);
        string Renew(string tokenStr, TokenClaim claim);
        IEnumerable<TokenClaim> GetClaims(string tokenStr);
        DateTime? GetExpirationDate(string tokenStr);
    }

    internal class Saml2TokenProvider: ITokenProvider
    {
        private IAPISettings APISettings { get; set; }
        private IEncryptionManager EncryptionManager { get; set; }
        public Saml2TokenProvider(IOptions<APISettings> apiSettings, IEncryptionManager encryptionManager)
        {
            APISettings = apiSettings.Value;
            EncryptionManager = encryptionManager;
        }
        private static Saml2SecurityTokenHandler SamlTokenHandler = new Saml2SecurityTokenHandler();
        private static IEnumerable<TokenClaim> DefaultClaims = new TokenClaim[0];

        public string Create(IEnumerable<TokenClaim> claims)
            => Serialize(CreateSaml2SecurityToken(claims));
        public string Renew(string tokenStr, IEnumerable<TokenClaim> claims)
            => Deserialize(tokenStr) is Saml2SecurityToken saml2Token && IsValidToken(saml2Token) ? Serialize(RenewToken(saml2Token, claims)) : null;
        public string Renew(string tokenStr, TokenClaim claim)
            => Deserialize(tokenStr) is Saml2SecurityToken saml2Token && IsValidToken(saml2Token) ? Serialize(RenewToken(saml2Token, claim)) : null;
        public IEnumerable<TokenClaim> GetClaims(string tokenStr)
            => Deserialize(tokenStr) is Saml2SecurityToken saml2Token && IsValidToken(saml2Token) ? GetClaims(saml2Token) : DefaultClaims;
        public DateTime? GetExpirationDate(string tokenStr)
            => Deserialize(tokenStr) is Saml2SecurityToken saml2Token && IsValidToken(saml2Token) ? saml2Token.Assertion.Conditions.NotOnOrAfter : null;




        private Saml2SecurityToken CreateSaml2SecurityToken(IEnumerable<TokenClaim> claims)
        {
            Saml2SubjectConfirmationData confirmationData = new Saml2SubjectConfirmationData() { Address = APISettings.ConfirmationMethod };
            Saml2SubjectConfirmation subjectConfirmations = new Saml2SubjectConfirmation(new Uri(APISettings.ConfirmationMethod), confirmationData);
            Saml2AudienceRestriction[] audienceRestriction = new Saml2AudienceRestriction[1] { new Saml2AudienceRestriction(APISettings.AudienceUri.ToString()) };
            Saml2Assertion assertion = new Saml2Assertion(new Saml2NameIdentifier(APISettings.Issuer))
            {
                Conditions = new Saml2Conditions(audienceRestriction)
                {
                    NotBefore = null,
                    NotOnOrAfter = null
                },
                InclusiveNamespacesPrefixList = APISettings.Namespace,
                Subject = new Saml2Subject(subjectConfirmations)
                {
                    NameId = new Saml2NameIdentifier(APISettings.ConfirmationMethod)
                }
            };


            return RenewToken(new Saml2SecurityToken(assertion), claims);
        }
        
        private bool IsValidToken(Saml2SecurityToken saml2Token)
        {
            DateTime utcNow = DateTime.UtcNow;
            bool isValidIssueDate = saml2Token.Assertion.Conditions.NotBefore is DateTime issueDate && issueDate <= utcNow;
            bool isValidExpirationDate = saml2Token.Assertion.Conditions.NotOnOrAfter is DateTime expirationDate && expirationDate > utcNow;

            return isValidIssueDate && isValidExpirationDate && APISettings.Issuer == saml2Token.Assertion.Issuer.Value;
        }
        private IEnumerable<TokenClaim> GetClaims(Saml2SecurityToken saml2Token)
            => saml2Token.Assertion.Statements.First() is Saml2AttributeStatement saml2AttributeStatement ?
            saml2AttributeStatement.Attributes.Select(a => new TokenClaim(a.Name, a.Values)) : DefaultClaims;



        private Saml2SecurityToken RenewToken(Saml2SecurityToken saml2Token, IEnumerable<TokenClaim> claims)
        {
            int validForFinal = APISettings.DefaultValidFor;
            DateTime utcNow = DateTime.UtcNow;
            saml2Token.Assertion.Conditions.NotBefore = utcNow;
            saml2Token.Assertion.Conditions.NotOnOrAfter = utcNow.AddMinutes(validForFinal);

            foreach (TokenClaim claim in claims ?? DefaultClaims)
            {
                AddUpdateSaml2Attribute(saml2Token, claim.Name, claim.GetValue());
            }
            return saml2Token;
        }
        private Saml2SecurityToken RenewToken(Saml2SecurityToken saml2Token, TokenClaim claim)
        {
            int validForFinal = APISettings.DefaultValidFor;
            DateTime utcNow = DateTime.UtcNow;
            saml2Token.Assertion.Conditions.NotBefore = utcNow;
            saml2Token.Assertion.Conditions.NotOnOrAfter = utcNow.AddMinutes(validForFinal);
            AddUpdateSaml2Attribute(saml2Token, claim.Name, claim.GetValue());
            return saml2Token;
        }

        private string Serialize(Saml2SecurityToken token)
        {
            var sw = new System.IO.StringWriter();
            using (var xmlWriter = new System.Xml.XmlTextWriter(sw))
            {
                SamlTokenHandler.WriteToken(xmlWriter, token);
                return EncryptionManager.Encrypt(sw.ToString());
            }

        }
        private Saml2SecurityToken Deserialize(string tokenStr)
        {
            string decryptedValue = EncryptionManager.Decrypt(tokenStr);
            return SamlTokenHandler.CanReadToken(decryptedValue) ? SamlTokenHandler.ReadSaml2Token(decryptedValue) : null;
        }
        
        private void AddUpdateSaml2Attribute(Saml2SecurityToken saml2Token, string name, object value)
        {
            if (!(saml2Token.Assertion.Statements.FirstOrDefault() is Saml2AttributeStatement))
            {
                saml2Token.Assertion.Statements.Add(new Saml2AttributeStatement());
            }
            Saml2AttributeStatement saml2AttributeStatement = saml2Token.Assertion.Statements.First() as Saml2AttributeStatement;
            saml2AttributeStatement.Attributes.Remove(saml2AttributeStatement.Attributes.FirstOrDefault(saml2Attribute => saml2Attribute.Name == name));
            Saml2Attribute saml2AttributeReplacement = value is IEnumerable<string> stringEnumerableValue ? new Saml2Attribute(name, stringEnumerableValue) : new Saml2Attribute(name, value.ToString());
            saml2AttributeStatement.Attributes.Add(saml2AttributeReplacement);
        }
        private IEnumerable<string> GetSaml2AttributeValue(Saml2SecurityToken saml2Token, string name)
        {
            if (saml2Token.Assertion.Statements.First() is Saml2AttributeStatement saml2AttributeStatement)
            {
                return saml2AttributeStatement.Attributes.FirstOrDefault(saml2Attribute => saml2Attribute.Name == name).Values;
            }
            return null;
        }
    }
}
