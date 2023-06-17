using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MQTT_Api_Server_Lifesaver.Extention
{
    public class Result
    {
        public ResultCodeEnum Code { get; set; }

        public bool Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public static Result Expire(ResultCodeEnum Code, string msg = "")
        {
            return new Result() { Code = Code, Status = false, Message = Get(msg, "token_expiration") };
        }
        public static Result Error(string msg = "")
        {
            return new Result() { Code = ResultCodeEnum.NotSuccess, Status = false, Message = Get(msg, "fail") };
        }
        public static Result Success(string msg = "")
        {
            return new Result() { Code = ResultCodeEnum.Success, Status = true, Message = Get(msg, "succeed") };
        }
        public static Result SuccessError(string msg = "")
        {
            return new Result() { Code = ResultCodeEnum.Success, Status = false, Message = Get(msg, "fail") };
        }


        public static Result UnAuthorize(string msg = "")
        {
            return new Result() { Code = ResultCodeEnum.NoPermission, Status = false, Message = Get(msg, "unAuthorize") };
        }
        public Result SetStatus(bool _Status)
        {
            if (_Status)
            {
                Code = ResultCodeEnum.Success;
                Message = "操作成功";
            }
            else
            {
                Code = ResultCodeEnum.NotSuccess;
                Message = "操作失败";
            }
            Status = _Status;
            return this;
        }
        public Result SetData(object? obj)
        {
            Data = obj;
            return this;
        }
        public Result SetCode(ResultCodeEnum Code)
        {
            this.Code = Code;
            return this;
        }
        public Result StatusFalse()
        {
            Status = false;
            return this;
        }
        public Result StatusTrue()
        {
            Status = true;
            return this;
        }

        public static string Get(string msg, string msg2)
        {
            if (msg == "")
            {
                msg = msg2;
            }
            return msg;
        }
    }
    public class Result<T>
    {
        public ResultCodeEnum Code { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public static Result<T> Error(string msg = "fail")
        {
            return new Result<T>() { Code = ResultCodeEnum.NotSuccess, Message = msg };
        }
        public static Result<T> Success(string msg = "succeed")
        {
            return new Result<T>() { Code = ResultCodeEnum.Success, Message = msg };
        }
        public static Result<T> UnAuthorize(string msg = "unAuthorize")
        {
            return new Result<T>() { Code = ResultCodeEnum.NoPermission, Message = msg };
        }

        public Result<T> SetData(T TValue)
        {
            Data = TValue;
            return this;
        }

        public Result<T> SetCode(ResultCodeEnum Code)
        {
            this.Code = Code;
            return this;
        }
    }
    public enum ResultCodeEnum
    {
        /// <summary>
        /// 操作成功。
        /// </summary>
        Success = 200,

        /// <summary>
        /// 操作不成功
        /// </summary>
        NotSuccess = 500,

        /// <summary>
        /// 无权限
        /// </summary>
        NoPermission = 401,

        /// <summary>
        ///  Access过期
        /// </summary>
        AccessTokenExpire = 1001,

        /// <summary>
        /// Refresh过期
        /// </summary>
        RefreshTokenExpire = 1002,

        /// <summary>
        /// 没有角色登录
        /// </summary>
        NoRoleLogin = 1003,
    }


    /// <summary>
    /// 异常抓取反馈扩展
    /// </summary>
    public class ErrorHandExtension
    {
        private readonly RequestDelegate next;
        public ErrorHandExtension(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (InvalidApiRequestException ex)
            {
                var statusCode = context.Response.StatusCode;
                context.Response.StatusCode = 500;
                await HandleExceptionAsync(context, statusCode, ex.Message);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                context.Response.StatusCode = 500;
                await HandleExceptionAsync(context, statusCode, ex.Message);
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                var msg = "";

                switch (statusCode)
                {
                    case 401: msg = "权限不足"; break;
                    case 403: msg = "未授权"; break;
                    case 404: msg = "未找到服务"; break;
                    case 502: msg = "请求错误"; break;
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    await HandleExceptionAsync(context, statusCode, msg);
                }
            }
        }
        //异常错误信息捕获，将错误信息用Json方式返回
        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string msg, ResultCodeEnum code = ResultCodeEnum.NotSuccess)
        {
            Result result;
            if (statusCode == 401)
            {
                result = Result.UnAuthorize(msg);
            }
            else
            {
                result = Result.Error(msg).SetCode(code);
            }

            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
