using DbFacade.DataLayer.Models;
using DbFacade.Exceptions;
using ServiceLayer.DomainLayer.DbMethods;
using ServiceLayer.DomainLayer.Models.Data;
using ServiceProvider.Services;
using System;

namespace ServiceLayer.DomainLayer
{
    public interface IDomainFacade
    {
        IDbResponse<TestUser> GetTestUsers();
    }
    internal class DomainFacadeBase: Service
    {
        protected override void Init() { }

        protected virtual void HandleSQLExecutionException(SQLExecutionException sqlEx) { throw new Exception("SQL Execution Exception", sqlEx); }
        protected virtual void HandleValidationException<TDbParamsModel>(ValidationException<TDbParamsModel> validationEx) where TDbParamsModel : DbParamsModel { throw new Exception("Validation Exception", validationEx); }
        protected IDbResponse<TDbDataModel> Run<TDbParamsModel,TDbDataModel>(Func<TDbParamsModel, IDbResponse<TDbDataModel>> methodHandler, TDbParamsModel model)
            where TDbParamsModel : DbParamsModel
            where TDbDataModel : DbDataModel
        {
            try
            {
                return methodHandler(model);
            }
            catch (SQLExecutionException sqlEx)
            {
                HandleSQLExecutionException(sqlEx);
                return default;
            }
            catch (ValidationException<TDbParamsModel> validationEx)
            {
                HandleValidationException<TDbParamsModel>(validationEx);
                return default;
            }

        }
        protected IDbResponse Run<TDbParamsModel>(Func<TDbParamsModel, IDbResponse> methodHandler, TDbParamsModel model)
            where TDbParamsModel : DbParamsModel
        {
            try
            {
                return methodHandler(model);
            }
            catch (SQLExecutionException sqlEx)
            {
                HandleSQLExecutionException(sqlEx);
                return default;
            }
            catch (ValidationException<TDbParamsModel> validationEx)
            {
                HandleValidationException<TDbParamsModel>(validationEx);
                return default;
            }

        }
        protected IDbResponse<TDbDataModel> Run<TDbDataModel>(Func<IDbResponse<TDbDataModel>> methodHandler) 
            where TDbDataModel: DbDataModel 
        {
            try
            {
                return methodHandler();
            }
            catch(SQLExecutionException sqlEx)
            {
                HandleSQLExecutionException(sqlEx);
                return default;
            }
            
        }
        protected IDbResponse Run<TDbDataModel>(Func<IDbResponse> methodHandler)
            where TDbDataModel : DbDataModel
        {
            try
            {
                return methodHandler();
            }
            catch (SQLExecutionException sqlEx)
            {
                HandleSQLExecutionException(sqlEx);
                return default;
            }

        }

        
    }
    internal class DomainFacade : DomainFacadeBase, IDomainFacade
    {
        public IDbResponse<TestUser> GetTestUsers() => Run(ConsumingApplicationDbMethods.GetTestUsers.Execute);
    }

}
