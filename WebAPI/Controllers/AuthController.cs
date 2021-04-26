using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.Database.Generation;
using WebAPI.Database.Hashing;
using WebAPI.Database.Interaction;
using WebAPI.Database.Validation;
using WebAPI.Models.DatabaseTableClasses;

namespace WebAPI.Controllers
{
    public class AuthCreateController : ApiController
    {
        [Route("api/auth/create")]

        public IHttpActionResult Get(string name, string email, string password)
        {
            if (!ValidationData.ValidationName(name) || !ValidationData.ValidationEmail(email) || !ValidationData.ValidationPassword(password))
                return InternalServerError(new Exception("name, email or password failed verification."));
            try
            {
                int count = 0;
                string token = null, refresh_token = null;
                do
                {
                    token = GenerationToken.Get(15);
                    refresh_token = GenerationToken.Get(6);
                    if (++count == 40) return InternalServerError(new Exception("Failed to generate token or refresh_token, try again."));
                } while (new PetaPoco.Database("DatabaseConnect").ExecuteScalar<bool>($"select count(*) from auth where token=@token", new { token }) &&
                         new PetaPoco.Database("DatabaseConnect").ExecuteScalar<bool>($"select count(*) from auth where refresh_token=@refresh_token", new { refresh_token }));
                if (InteractionDatabase.Uniqueness("auth", "email", email))
                {
                    if (InteractionDatabase.Add(
                        table: "auth",
                        columns: "token, refresh_token, name, email, password",
                        data: $"'{token}', '{refresh_token}' , '{name}', '{email}', '{PasswordHashing.GetPasswordHashing(password)}'"
                    )) return Ok("Auth created.");
                    else return InternalServerError(new Exception("Internal server error."));
                }
                else return InternalServerError(new Exception("This email already exists."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }

    public class AuthGetTokenController : ApiController
    {
        private class JsonAuthTokens
        { 
            public string token { get; set; }
            public string refresh_token { get; set; }
        }

        [Route("api/auth/gettoken")]
        public IHttpActionResult Get(string email, string password)
        {
            if (!ValidationData.Valid(email) || !ValidationData.Valid(password))
                return InternalServerError(new Exception("email or password failed verification."));
            try
            {
                IEnumerable<Auth> auth = new PetaPoco.Database("DatabaseConnect").Fetch<Auth>($"select * from auth where email in ('{email}')", new { email });
                if (auth.ToList().Count > 0 && auth.ToList()[0].email == email && auth.ToList()[0].password == PasswordHashing.GetPasswordHashing(password)) 
                    return Json(new JsonAuthTokens { token = auth.ToList()[0].token, refresh_token = auth.ToList()[0].refresh_token});
                else return InternalServerError(new Exception("Internal server error."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }

    public class AuthUpdateTokenController : ApiController
    {
        private class JsonAuthToken
        {
            public string token { get; set; }
        }

        [Route("api/auth/updatetoken")]
        public IHttpActionResult Get(string refresh_token)
        {
            if (!ValidationData.Valid(refresh_token))
                return InternalServerError(new Exception("refresh_token failed verification."));
            try
            {
                if (!InteractionDatabase.Uniqueness("auth", "refresh_token", refresh_token))
                {
                    new PetaPoco.Database("DatabaseConnect").Execute($"update auth set token = '{GenerationToken.Get(15)}' where refresh_token = '{refresh_token}'");
                    IEnumerable<Auth> auth = new PetaPoco.Database("DatabaseConnect").Fetch<Auth>($"select * from auth where refresh_token in ('{refresh_token}')", new { refresh_token });
                    if (auth.ToList().Count > 0 && auth.ToList()[0].refresh_token == refresh_token)
                        return Json(new JsonAuthToken { token = auth.ToList()[0].token });
                    else return InternalServerError(new Exception("Internal server error."));
                }
                else return InternalServerError(new Exception("refresh_token not found."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }
}
