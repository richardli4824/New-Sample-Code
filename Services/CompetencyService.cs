using Microsoft.AspNet.Identity;
using Sample.Data;
using Sample.Domain;
using Sample.Models.Requests;
using Sample.Models.Responses;
using Sample.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sample.Services
{
    public class CompetencyService : BaseService, ICompetencyService
    {
        //get all competency users. this is used to populate the index page.
        //some dir-paginate calls in here that do not work yet.
        public PaginateItemsResponse<CompetencyUser> GetCompetencyUsers()
        {
            PaginatedRequest model = new PaginatedRequest();
            List<CompetencyUser> userList = null;
            PaginateItemsResponse<CompetencyUser> response = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Competency_Users_SelectAll"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                  paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);
                  paramCollection.AddWithValue("@Query", model.Query);


              }, map: delegate (IDataReader reader, short set)
              {

                  if (set == 0)
                  {
                      CompetencyUser CompetencyUser = new CompetencyUser();
                      int startingIndex = 0;

                      CompetencyUser.Id = reader.GetSafeString(startingIndex++);
                      CompetencyUser.FirstName = reader.GetSafeString(startingIndex++);
                      CompetencyUser.LastName = reader.GetSafeString(startingIndex++);
                      CompetencyUser.CompetencyId = reader.GetSafeInt32(startingIndex++);
                      CompetencyUser.CompetencyName = reader.GetSafeString(startingIndex++);
                      CompetencyUser.Email = reader.GetSafeString(startingIndex++);
                      CompetencyUser.AuthorizedBy = reader.GetSafeString(startingIndex++);
                      CompetencyUser.CreatedDate = reader.GetDateTime(startingIndex++);
                      CompetencyUser.Status = reader.GetSafeString(startingIndex++);
                      CompetencyUser.ActivityType = reader.GetSafeString(startingIndex++);
                      CompetencyUser.Notes = reader.GetSafeString(startingIndex++);
                      CompetencyUser.Attachments = reader.GetSafeString(startingIndex++);
                      CompetencyUser.competencyUserId = reader.GetSafeInt32(startingIndex++);


                      if (userList == null)
                      {
                          userList = new List<CompetencyUser>();
                      }
                      userList.Add(CompetencyUser);

                  }
                  else if (set == 1)
                  {

                      response = new PaginateItemsResponse<CompetencyUser>();

                      response.TotalItems = reader.GetSafeInt32(0);

                  }
              }

           );
            response.Items = userList;
            response.CurrentPage = model.CurrentPage;
            response.ItemsPerPage = model.ItemsPerPage;
            return response;

        }

        //get list of users with no competency
        public List<CompetencyUserNew> GetNewUsers()
        {
            List<CompetencyUserNew> List = new List<CompetencyUserNew>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Competency_Users_SelectNew"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {

            }, map: delegate (IDataReader reader, short set)
            {
                CompetencyUserNew CompetencyUserNew = new CompetencyUserNew();
                int startingIndex = 0;

                CompetencyUserNew.Id = reader.GetSafeString(startingIndex++);
                CompetencyUserNew.FirstName = reader.GetSafeString(startingIndex++);
                CompetencyUserNew.LastName = reader.GetSafeString(startingIndex++);
                CompetencyUserNew.Email = reader.GetSafeString(startingIndex++);

                List.Add(CompetencyUserNew);
            });

            return List;
        }

        //get list of competencies
        public List<Competencies> GetCompetencies()
        {
            List<Competencies> List = new List<Competencies>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Competencies_SelectAll"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {

            }, map: delegate (IDataReader reader, short set)
            {
                Competencies Competencies = new Competencies();
                int startingIndex = 0;

                Competencies.Id = reader.GetSafeInt32(startingIndex++);
                Competencies.Name = reader.GetSafeString(startingIndex++);


                List.Add(Competencies);
            });

            return List;
        }

        //inserts new competency user.
        public int NewCompetencyInsert(AdminCompetencyRequestModel model)
        {
            int uid = 0;
            string authorizedBy = HttpContext.Current.User.Identity.Name.ToString();
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Competency_Users_Insert"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@UserId", model.UserId);
                   paramCollection.AddWithValue("@CompetencyId", model.CompetencyId);
                   paramCollection.AddWithValue("@AuthorizedBy", authorizedBy);
                   paramCollection.AddWithValue("@Status", model.Status);
                   paramCollection.AddWithValue("@Notes", model.Notes);
                   paramCollection.AddWithValue("@ActivityType", model.ActivityType);


                   SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                   p.Direction = System.Data.ParameterDirection.Output;

                   paramCollection.Add(p);

               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   int.TryParse(param["@Id"].Value.ToString(), out uid);
               });

            return uid;
        }

        //get competency user by UserId
        public CompetencyUser GetCompetencyUserById(int UcId)
        {
            CompetencyUser User = new CompetencyUser();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Competency_Users_SelectById"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", UcId);
                }, map: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;

                    User.competencyUserId = reader.GetSafeInt32(startingIndex++);
                    User.Id = reader.GetSafeString(startingIndex++);
                    User.FirstName = reader.GetSafeString(startingIndex++);
                    User.LastName = reader.GetSafeString(startingIndex++);
                    User.CompetencyId = reader.GetSafeInt32(startingIndex++);
                    User.Status = reader.GetSafeString(startingIndex++);
                    User.Notes = reader.GetSafeString(startingIndex++);
                    User.ActivityType = reader.GetSafeString(startingIndex++);
                    User.Attachments = reader.GetSafeString(startingIndex++);

                });

            return User;
        }

        //update competency user
        public void Update(AdminCompetencyRequestModel model)
        {

            string authorizedBy = HttpContext.Current.User.Identity.Name.ToString();
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Competency_Users_Update"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", model.competencyUserId);
                    paramCollection.AddWithValue("@UserId", model.Id);
                    paramCollection.AddWithValue("@CompetencyId", model.CompetencyId);
                    paramCollection.AddWithValue("@AuthorizedBy", authorizedBy);
                    paramCollection.AddWithValue("@Status", model.Status);
                    paramCollection.AddWithValue("@Notes", model.Notes);
                    paramCollection.AddWithValue("@ActivityType", model.ActivityType);
                    paramCollection.AddWithValue("@Attachment", model.Attachment);

                });
        }

        //delete competency user
        public void Delete(int CompetencyUserId)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Competency_Users_Delete"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", CompetencyUserId);

                });
        }

        public CompetencyUser GetCompetencyUserByUserId(string userId)
        {
            CompetencyUser User = new CompetencyUser();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Competency_Users_SelectByUserId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", userId);
                }, map: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;

                    User.competencyUserId = reader.GetSafeInt32(startingIndex++);
                    User.Id = reader.GetSafeString(startingIndex++);
                    User.FirstName = reader.GetSafeString(startingIndex++);
                    User.LastName = reader.GetSafeString(startingIndex++);
                    User.CompetencyId = reader.GetSafeInt32(startingIndex++);
                    User.Status = reader.GetSafeString(startingIndex++);
                    User.Notes = reader.GetSafeString(startingIndex++);
                    User.ActivityType = reader.GetSafeString(startingIndex++);
                    User.Attachments = reader.GetSafeString(startingIndex++);

                });

            return User;
        }


        
    }
}