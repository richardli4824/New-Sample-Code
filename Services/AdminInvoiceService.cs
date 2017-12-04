using Microsoft.Practices.Unity;
using Sample.Data;
using Sample.Domain;
using Sample.Models.Requests;
using Sample.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sample.Services
{
    public class AdminInvoiceService : BaseService, IAdminInvoiceService
    {
        public int InvoiceInsert(AdminInvoiceRequestModel model)
        {
            int uid = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Invoice_Insert"
           , inputParamMapper: delegate (SqlParameterCollection paramCollection)
           {
               paramCollection.AddWithValue("@ClientId", model.ClientId);
               paramCollection.AddWithValue("@QuoteAccepted", model.QuoteAccepted);
               paramCollection.AddWithValue("@Price", model.Price);
               paramCollection.AddWithValue("@FileNumber", model.FileNumber);
               paramCollection.AddWithValue("@ProductDescription", model.ProductDescription);


               SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
               p.Direction = System.Data.ParameterDirection.Output;

               paramCollection.Add(p);

           }, returnParameters: delegate (SqlParameterCollection param)
           {
               int.TryParse(param["@Id"].Value.ToString(), out uid);
           });
            InsertInvoiceLineItems(uid, model.LineItems);

            return uid;
        }

        public void InsertInvoiceLineItems(int id, List<InvoiceLineItems> List)
        {
            foreach (InvoiceLineItems item in List)
            {
             DataProvider.ExecuteNonQuery(GetConnection, "dbo.InvoiceLineItems_Insert"
           , inputParamMapper: delegate (SqlParameterCollection paramCollection)
           {
               decimal lineItemPrice = item.UnitPrice * item.Quantity;

               paramCollection.AddWithValue("@InvoiceId", id);
               paramCollection.AddWithValue("@Name", item.Name);
               paramCollection.AddWithValue("@FeeStructureId", item.FeeStructureId);
               paramCollection.AddWithValue("@LineItemPrice", lineItemPrice);
               paramCollection.AddWithValue("@UnitPrice", item.UnitPrice);
               paramCollection.AddWithValue("@Quantity", item.Quantity);

           }, returnParameters: delegate (SqlParameterCollection param)
           {
           });
            }
        }

        public void UpdateInvoice(AdminInvoiceRequestModel model)
        {

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Invoice_Update"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", model.Id);
                    paramCollection.AddWithValue("@ClientId", model.ClientId);
                    paramCollection.AddWithValue("@QuoteAccepted", model.QuoteAccepted);
                    paramCollection.AddWithValue("@Price", model.Price);

                });
            UpdateInvoiceLineItems(model.Id, model.LineItems);
        }

        public void UpdateInvoiceLineItems(int id, List<InvoiceLineItems> List)
        {
            foreach (InvoiceLineItems item in List)
            {
                if (item.LineItemId != 0)
                {
                    DataProvider.ExecuteNonQuery(GetConnection, "dbo.InvoiceLineItems_Update"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@InvoiceId", id);
                      paramCollection.AddWithValue("@Name", item.Name);
                      paramCollection.AddWithValue("@FeeStructureId", item.FeeStructureId);
                      paramCollection.AddWithValue("@LineItemPrice", item.LineItemPrice);
                      paramCollection.AddWithValue("@UnitPrice", item.UnitPrice);
                      paramCollection.AddWithValue("@Quantity", item.Quantity);
                      paramCollection.AddWithValue("@LineItemId", item.LineItemId);
                  });
                }
                else
                {
                    DataProvider.ExecuteNonQuery(GetConnection, "dbo.InvoiceLineItems_Insert"
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@InvoiceId", id);
                      paramCollection.AddWithValue("@Name", item.Name);
                      paramCollection.AddWithValue("@FeeStructureId", item.FeeStructureId);
                      paramCollection.AddWithValue("@LineItemPrice", item.LineItemPrice);
                      paramCollection.AddWithValue("@UnitPrice", item.UnitPrice);
                      paramCollection.AddWithValue("@Quantity", item.Quantity);

                  }, returnParameters: delegate (SqlParameterCollection param)
                  {
                  });
                }
            }
        }

        public int QuoteRevisedInsert(AdminInvoiceRequestModel model)
        {
            int uid = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.InvoiceRevisedQuote_Insert"
           , inputParamMapper: delegate (SqlParameterCollection paramCollection)
           {
               paramCollection.AddWithValue("@ClientId", model.ClientId);
               paramCollection.AddWithValue("@QuoteAccepted", model.QuoteAccepted);
               paramCollection.AddWithValue("@Price", model.Price);

               SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
               p.Direction = System.Data.ParameterDirection.Output;

               paramCollection.Add(p);

           }, returnParameters: delegate (SqlParameterCollection param)
           {
               int.TryParse(param["@Id"].Value.ToString(), out uid);
           });
            InsertInvoiceLineItems(uid, model.LineItems);
            QuoteRevisedUpdateStatus(uid, model);

            return uid;
        }

        public void QuoteRevisedUpdateStatus(int Id, AdminInvoiceRequestModel model)
        {

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.InvoiceRevisedQuote_Update"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", model.CurrentInvoice);
                    paramCollection.AddWithValue("@Status", model.Status);
                    paramCollection.AddWithValue("@ReplacedBy", Id);

                });
        }

        public List<Quote> GetQuoteByClientId(int ClientId)
        {
            List<Quote> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.InvoiceQuotes_SelectByClientId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@ClientId", ClientId);

                }, map: delegate (IDataReader reader, short set)
                {
                    Quote q = new Quote();
                    int startingIndex = 0;

                    q.Id = reader.GetSafeInt32(startingIndex++);
                    q.ClientId = reader.GetSafeInt32(startingIndex++);
                    q.QuoteAccepted = reader.GetSafeInt32(startingIndex++);
                    q.Price = reader.GetSafeDecimal(startingIndex++);

                    if (list == null)
                    {
                        list = new List<Quote>();
                    }

                    list.Add(q);

                });

            return list;
        }

        public List<Quote> GetInvoiceByClientId(int ClientId)
        {
            List<Quote> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Invoices_SelectByClientId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@ClientId", ClientId);

                }, map: delegate (IDataReader reader, short set)
                {
                    Quote q = new Quote();
                    int startingIndex = 0;

                    q.Id = reader.GetSafeInt32(startingIndex++);
                    q.ClientId = reader.GetSafeInt32(startingIndex++);
                    q.QuoteAccepted = reader.GetSafeInt32(startingIndex++);
                    q.Price = reader.GetSafeDecimal(startingIndex++);

                    if (list == null)
                    {
                        list = new List<Quote>();
                    }

                    list.Add(q);

                });

            return list;
        }

        public Quote GetQuoteByQuoteId(int ClientId, int QuoteId)
        {
            Quote q = new Quote();

            DataProvider.ExecuteCmd(GetConnection, "dbo.InvoiceQuotes_SelectByQuoteId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@ClientId", ClientId);
                    paramCollection.AddWithValue("@Id", QuoteId);

                }, map: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;

                    q.Id = reader.GetSafeInt32(startingIndex++);
                    q.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                    q.ClientId = reader.GetSafeInt32(startingIndex++);
                    q.QuoteAccepted = reader.GetSafeInt32(startingIndex++);
                    q.Price = reader.GetSafeDecimal(startingIndex++);
                    q.FileNumber = reader.GetSafeString(startingIndex++);
                    q.ProductDescription = reader.GetSafeString(startingIndex++);

                });

            return q;
        }

        public void AcceptQuote(AdminInvoiceRequestModel model)
        {

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.InvoiceQuotes_Update"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", model.Id);
                    paramCollection.AddWithValue("@QuoteAccepted", model.QuoteAccepted);

                });
        }


        public List<Invoice> GetFeeStructureByProjectTypeId(int ProjectTypeId)
        {
            List<Invoice> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.ProjectType_InvoiceFeeStructures_Xref_SelectInvoiceFeeByProjectId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@ProjectId", ProjectTypeId);

                }, map: delegate (IDataReader reader, short set)
                {
                    Invoice q = new Invoice();
                    int startingIndex = 0;

                    q.FileType = reader.GetSafeString(startingIndex++);
                    q.Name = reader.GetSafeString(startingIndex++);
                    q.UnitPrice = reader.GetSafeDecimal(startingIndex++);
                    q.InvoicesProjectTypeId = reader.GetSafeInt32(startingIndex++);
                    q.FeeStructureId = reader.GetSafeInt32(startingIndex++);

                    if (list == null)
                    {
                        list = new List<Invoice>();
                    }

                    list.Add(q);

                });

            return list;
        }

        public List<Invoice> GetLineItemByInvoiceId(int InvoiceId)
        {
            List<Invoice> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.InvoiceLineItems_SelectByInvoiceId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", InvoiceId);

                }, map: delegate (IDataReader reader, short set)
                {
                    Invoice invoice = new Invoice();
                    int startingIndex = 0;

                    invoice.Id = reader.GetSafeInt32(startingIndex++);
                    invoice.Name = reader.GetSafeString(startingIndex++);
                    invoice.FeeStructureId = reader.GetSafeInt32(startingIndex++);
                    invoice.LineItemPrice = reader.GetSafeDecimal(startingIndex++);

                    if (list == null)
                    {
                        list = new List<Invoice>();
                    }

                    list.Add(invoice);

                });

            return list;
        }

        public List<InvoiceProjectType> GetInvoiceProjectTypes()
        {
            List<InvoiceProjectType> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.InvoiceProjectTypes_SelectAll"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {

                }, map: delegate (IDataReader reader, short set)
                {
                    InvoiceProjectType p = new InvoiceProjectType();
                    int startingIndex = 0;

                    p.Id = reader.GetSafeInt32(startingIndex++);
                    p.FileType = reader.GetSafeString(startingIndex++);

                    if (list == null)
                    {
                        list = new List<InvoiceProjectType>();
                    }

                    list.Add(p);

                });

            return list;
        }

        public List<Invoice> GetInvoices()
        {
            List<Invoice> List = new List<Invoice>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Invoices_SelectAll"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {

            }, map: delegate (IDataReader reader, short set)
            {
                Invoice Invoices = new Invoice();
                int startingIndex = 0;

                Invoices.Id = reader.GetSafeInt32(startingIndex++);
                Invoices.QuoteAccepted = reader.GetSafeInt32(startingIndex++);
                Invoices.UnitPrice = reader.GetSafeDecimal(startingIndex++);
                Invoices.ClientId = reader.GetSafeInt32(startingIndex++);
                Invoices.Name = reader.GetSafeString(startingIndex++);
                Invoices.Phone = reader.GetSafeString(startingIndex++);
                Invoices.Email = reader.GetSafeString(startingIndex++);
                Invoices.Status = reader.GetSafeString(startingIndex++);


                List.Add(Invoices);
            });

            return List;
        }

        public List<Invoice> GetInvoiceByInvoiceId(int InvoiceId)
        {
            List<Invoice> List = new List<Invoice>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Invoice_SelectByInvoiceId"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", InvoiceId);

            }, map: delegate (IDataReader reader, short set)
            {
                Invoice Invoices = new Invoice();
                int startingIndex = 0;

                Invoices.Id = reader.GetSafeInt32(startingIndex++);
                Invoices.ClientId = reader.GetSafeInt32(startingIndex++);
                Invoices.QuoteAccepted = reader.GetSafeInt32(startingIndex++);
                Invoices.Price = reader.GetSafeDecimal(startingIndex++);
                Invoices.Name = reader.GetSafeString(startingIndex++);
                Invoices.Client = reader.GetSafeString(startingIndex++);
                Invoices.LineItemPrice = reader.GetSafeDecimal(startingIndex++);
                Invoices.UnitPrice = reader.GetSafeDecimal(startingIndex++);
                Invoices.FeeStructureId = reader.GetSafeInt32(startingIndex++);
                Invoices.Phone = reader.GetSafeString(startingIndex++);
                Invoices.Email = reader.GetSafeString(startingIndex++);
                Invoices.Quantity = reader.GetSafeInt32(startingIndex++);
                Invoices.LineItemId = reader.GetSafeInt32(startingIndex++);
                Invoices.Status = reader.GetSafeString(startingIndex++);
                Invoices.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                Invoices.FileNumber = reader.GetSafeString(startingIndex++);
                Invoices.ProductDescription = reader.GetSafeString(startingIndex++);

                List.Add(Invoices);
            });

            return List;
        }

        public List<Invoice> GetRevisedInvoices(int InvoiceId)
        {
            List<Invoice> List = new List<Invoice>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.InvoiceQuotes_SelectAllByRevised"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", InvoiceId);

            }, map: delegate (IDataReader reader, short set)
            {
                Invoice Invoices = new Invoice();
                int startingIndex = 0;

                Invoices.Id = reader.GetSafeInt32(startingIndex++);
                Invoices.ClientId = reader.GetSafeInt32(startingIndex++);
                Invoices.QuoteAccepted = reader.GetSafeInt32(startingIndex++);
                Invoices.Price = reader.GetSafeDecimal(startingIndex++);
                Invoices.Status = reader.GetSafeString(startingIndex++);
                Invoices.ReplacedBy = reader.GetSafeInt32(startingIndex++);


                List.Add(Invoices);
            });

            return List;
        }

        public int InvoiceCommentInsert(AdminInvoiceCommentServiceModel model)
        {
            int cid = 0;

            string currentUserId = UserService.GetCurrentUserId();
            
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_Insert"
           , inputParamMapper: delegate (SqlParameterCollection paramCollection)
           {
               paramCollection.AddWithValue("@Comment", model.CommentText);
               paramCollection.AddWithValue("@UserID", currentUserId);
               paramCollection.AddWithValue("@ServiceID", commentServiceId);


               SqlParameter p = new SqlParameter("@CommentID", System.Data.SqlDbType.Int);
               p.Direction = System.Data.ParameterDirection.Output;

               paramCollection.Add(p);

           }, returnParameters: delegate (SqlParameterCollection param)
           {
               int.TryParse(param["@CommentID"].Value.ToString(), out cid);
           });
            InsertInvoiceCommentXref(cid, model.InvoiceId, model.DisplayToCLient);

            return cid;
        }

        public void InsertInvoiceCommentXref(int id, int invoiceId, int displayToCLient)
        {
                DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_InvoiceCommentsXrefInsert"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {

                  paramCollection.AddWithValue("@InvoiceID", invoiceId);
                  paramCollection.AddWithValue("@CommentId", id);
                  paramCollection.AddWithValue("@DisplayToClient", displayToCLient);

              }, returnParameters: delegate (SqlParameterCollection param)
              {
              });
        }

        public List<InvoiceComment> GetCommentByInvoiceId(int InvoiceId)
        {
            List<InvoiceComment> List = new List<InvoiceComment>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Comments_GetInvoiceCommentsByInvoiceID"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@InvoiceId", InvoiceId);

            }, map: delegate (IDataReader reader, short set)
            {
                InvoiceComment Comment = new InvoiceComment();
                int startingIndex = 0;

                Comment.InvoiceId = reader.GetSafeInt32(startingIndex++);
                Comment.CommentId = reader.GetSafeInt32(startingIndex++);
                Comment.CommentText = reader.GetSafeString(startingIndex++);
                Comment.FirstName = reader.GetSafeString(startingIndex++);
                Comment.LastName = reader.GetSafeString(startingIndex++);
                Comment.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                Comment.DisplayToClient = reader.GetSafeInt32(startingIndex++);

                List.Add(Comment);
            });

            return List;
        }

        public void UpdateCommentDisplaySetting(InvoiceComment model)
        {

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Comments_InvoiceCommentsXrefUpdate"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@CommentID", model.CommentId);
                    paramCollection.AddWithValue("@DisplayToClient", model.DisplayToClient);
                });
        }

    }
}