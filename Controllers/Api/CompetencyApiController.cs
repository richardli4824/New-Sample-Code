using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Sample.Domain;
using Sample.Models.Requests;
using Sample.Models.Responses;
using Sample.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sample.Controllers.Api
{ 
    [RoutePrefix("api/admin/competency")]
    public class CompetencyApiController : ApiController
    {
        [Dependency]
        public ICompetencyService _CompetencyService { get; set; }

        [Route, HttpGet]
        public HttpResponseMessage GetCompetencyUsers()
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            PaginateItemsResponse<CompetencyUser> response = _CompetencyService.GetCompetencyUsers();

            return Request.CreateResponse(response);

        }

        [Route("new"), HttpGet]
        public HttpResponseMessage GetNewUsers()
        {
            ItemsResponse<CompetencyUserNew> response = new ItemsResponse<CompetencyUserNew>();

            response.Items = _CompetencyService.GetNewUsers();

            return Request.CreateResponse(response);

        }

        [Route("competencies"), HttpGet]
        public HttpResponseMessage GetCompetencies()
        {
            ItemsResponse<Competencies> response = new ItemsResponse<Competencies>();

            response.Items = _CompetencyService.GetCompetencies();

            return Request.CreateResponse(response);
        }

        [Route, HttpPost]
        public HttpResponseMessage Insert(AdminCompetencyRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();

            response.Item = _CompetencyService.NewCompetencyInsert(model);

            return Request.CreateResponse(response);
        }

        [Route("edit/{UcId:int}"), HttpGet]
        public HttpResponseMessage GetCompetencyUserById(int UcId)
        {
            ItemResponse<CompetencyUser> response = new ItemResponse<CompetencyUser>();

            response.Item = _CompetencyService.GetCompetencyUserById(UcId);

            return Request.CreateResponse(response);
        }

        [Route, HttpPut]
        public HttpResponseMessage Update(AdminCompetencyRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            SuccessResponse response = new SuccessResponse();

            _CompetencyService.Update(model);

            return Request.CreateResponse(response);
        }

        [Route("{CompetencyUserId:int}"), HttpDelete]
        public HttpResponseMessage Delete(int CompetencyUserId)
        {
            SuccessResponse response = new SuccessResponse();

            _CompetencyService.Delete(CompetencyUserId);

            return Request.CreateResponse(response);
        }
    }
}
