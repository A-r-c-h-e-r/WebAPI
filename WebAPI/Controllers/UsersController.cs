using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.Database.Interaction;
using WebAPI.Database.Validation;
using WebAPI.Models.DatabaseTableClasses;

namespace WebAPI.Controllers
{
    public class UsersCreateController : ApiController
    {
        [Route("api/users/create")]
        public IHttpActionResult Get(string token, string name, string email)
        {
            if (!ValidationData.ValidationName(name) || !ValidationData.ValidationEmail(email) || !ValidationData.Valid(token))
                return InternalServerError(new Exception("token, name or email failed verification."));
            try
            {
                if (InteractionDatabase.Uniqueness("users", "email", email) && InteractionDatabase.Uniqueness("users", "token", token))
                {
                    if (InteractionDatabase.Add(
                        table: "users",
                        columns: "token, name, email",
                        data: $"'{token}', '{name}', '{email}'"
                    )) return Ok("User created.");
                    else return InternalServerError(new Exception("Internal server error."));
                }
                else return InternalServerError(new Exception("This email or token already exists."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }

    public class UsersGetController : ApiController
    {
        class JsonUsers
        {
            public string name { get; set; }
            public string email { get; set; }
        }

        [Route("api/users/get")]
        public IHttpActionResult Get(string token, string email)
        {
            if (!ValidationData.Valid(email) || !ValidationData.Valid(token))
                return InternalServerError(new Exception("token or email failed verification."));
            try
            {
                IEnumerable<Users> users = new PetaPoco.Database("DatabaseConnect").Fetch<Users>
                    ($"select * from users where email in ('{email}') and token in ('{token}')", new { email });
                if (users.ToList().Count > 0)
                    return Json(new JsonUsers { name = users.ToList()[0].name, email = users.ToList()[0].email });
                else return InternalServerError(new Exception("Internal server error."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }

    public class UsersDeleteController : ApiController
    {
        [Route("api/users/delete")]
        public IHttpActionResult Get(string token, string email)
        {
            if (!ValidationData.Valid(email) || !ValidationData.Valid(token))
                return InternalServerError(new Exception("token or email failed verification."));
            try
            {
                IEnumerable<Users> users = new PetaPoco.Database("DatabaseConnect").Fetch<Users>($"select * from users where email in ('{email}') and token in ('{token}')", new { email });
                if (users.ToList().Count > 0)
                {
                    new PetaPoco.Database("DatabaseConnect").Execute($"delete from users where token = '{token}' and email = '{email}'");
                    return Ok("User deleted.");
                }
                else return InternalServerError(new Exception("Internal server error."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }
 
    public class UsersEditController : ApiController
    {
        [Route("api/users/edit")]
        public IHttpActionResult Get(string token, string name, string email)
        {
            if (!ValidationData.ValidationName(name) || !ValidationData.ValidationEmail(email) || !ValidationData.Valid(token))
                return InternalServerError(new Exception("token, name or email failed verification."));
            try
            {
                IEnumerable<Users> users = new PetaPoco.Database("DatabaseConnect").Fetch<Users>($"select * from users where email in ('{email}') and token in ('{token}')", new { email });
                if (users.ToList().Count > 0)
                {
                    new PetaPoco.Database("DatabaseConnect").Execute($"update users set name = '{name}' where token = '{token}' and email = '{email}'");
                    return Ok("User name edit.");
                }
                else return InternalServerError(new Exception("Internal server error."));
            }
            catch (Exception exception) { return InternalServerError(exception); }
        }
    }
}