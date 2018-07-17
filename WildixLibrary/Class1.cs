using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace WildixLibrary
{
    public class WildixAPI_Methods
    {
        // Delete everything from phonebook
        public static string Wildix_Delete_Clear_Phonebook(RestClient restclient, string phonebook_id)
        {
            var request = new RestRequest("api/v1/Phonebooks/" + phonebook_id + "/Contacts/", Method.DELETE);
            request.AddHeader("header", "application/x-www-form-urlencoded");
            IRestResponse response = restclient.Execute(request);
            return response.Content;
        }

        // Insert a contact
        public static string Wildix_Insert_Contacts_in_Phonebook(RestClient restclient, string phonebook_id, List<WildixContact> contact_list)
        {
            try
            {
                foreach (WildixContact contact in contact_list)
                {
                    var req = new RestRequest("api/v1/Phonebooks/" + phonebook_id + "/Contacts/", Method.POST);
                    var request = Wildix_Contact_Request_Build(req, contact);
                    IRestResponse response = restclient.Execute(request);
                }
            }
            catch
            {
                return "FAIL";
            }

            return "OK";
        }

        // Update a contact
        public static string Wildix_Update_Contacts_in_Phonebook(RestClient restclient, string phonebook_id, List<WildixContact> contact_list, int first_contact_id)
        {
            try
            {
                foreach (WildixContact contact in contact_list)
                {
                    var req = new RestRequest("api/v1/Phonebooks/" + phonebook_id + "/Contacts/" + first_contact_id, Method.PUT);
                    var request = Wildix_Contact_Request_Build(req, contact);
                    IRestResponse response = restclient.Execute(request);
                }
            }
            catch
            {
                return "FAIL";
            }
            return "OK";
        }

        // Get contacts from a phonebook
        public static List<WildixContact> Wildix_Get_Contacts_from_Phonebook(RestClient restclient, string phonebook_id)
        {
            List<WildixContact> Wildix_contacts = new List<WildixContact>();

            var requestContacts = new RestRequest("api/v1/Phonebooks/" + phonebook_id + "/Contacts/", Method.GET);
            requestContacts.AddHeader("header", "application/x-www-form-urlencoded");
            IRestResponse responseContacts = restclient.Execute(requestContacts);

            if (responseContacts.Content != "")
            {
                dynamic wobj = JObject.Parse(responseContacts.Content);

                foreach (dynamic wcontact in wobj.result.records)
                {
                    Wildix_contacts.Add(
                        new WildixContact
                        {
                            Id = wcontact.id.Value.ToString(),
                            Name = wcontact.name.Value.ToString(),
                            Email = wcontact.email.Value.ToString(),
                            Extension = wcontact.extension.Value.ToString(),
                            Phone = wcontact.phone.Value.ToString(),
                            Office = wcontact.office.Value.ToString(),
                            Home = wcontact.home.Value.ToString(),
                            Home_mobile = wcontact.home_mobile.Value.ToString(),
                            Fax = wcontact.fax.Value.ToString(),
                            Mobile = wcontact.mobile.Value.ToString(),
                            Organization = wcontact.organization.Value.ToString(),
                            Type = wcontact.type.Value.ToString(),
                            Address = wcontact.address.Value.ToString(),
                            vat_id = wcontact.vat_id.Value.ToString(),
                            Document_type = wcontact.document_type.Value.ToString(),
                            Document_id = wcontact.document_id.Value.ToString(),
                            Country = wcontact.country.Value.ToString(),
                            Date_birth = wcontact.date_birth.Value.ToString(),
                            Born_in = wcontact.born_in.Value.ToString(),
                            Postal_code = wcontact.postal_code.Value.ToString(),
                            City = wcontact.city.Value.ToString(),
                            Province = wcontact.province.Value.ToString(),
                            License_plate = wcontact.license_plate.Value.ToString(),
                            Document_released = wcontact.document_released.Value.ToString(),
                            Document_from = wcontact.document_from.Value.ToString(),
                            Phonebook_id = wcontact.phonebook_id.Value.ToString(),
                            Note = wcontact.note.Value.ToString(),
                            Pin = wcontact.pin.Value.ToString(),
                            Credit = wcontact.credit.Value.ToString(),
                            Picture = wcontact.picture.Value.ToString(),
                            Name_prefix = wcontact.name_prefix.Value.ToString(),
                            Shortcut = wcontact.shortcut.Value.ToString(),
                        }
                        );
                }
            }
            return Wildix_contacts;
        }

        // Update or Insert a record if already exists
        public static string Wildix_Upsert(RestClient restclient, string phonebook_id, List<WildixContact> contact_list)
        {
            var Already_present_contacts = Wildix_Get_Contacts_from_Phonebook(restclient, phonebook_id);
            
            int already_present_contacts_count = Already_present_contacts.Count();
            int total_contacts = contact_list.Count;
            var Update_List = new List<WildixContact>();
            var Insert_List = new List<WildixContact>();
            int first_contact_id = Already_present_contacts.First().Id;


            Update_List = contact_list.GetRange(0, already_present_contacts_count);
            contact_list.RemoveRange(0, already_present_contacts_count);
            Insert_List = contact_list;
            contact_list = null;

            try
            {
                foreach (WildixContact contact in Update_List)
                {
                    Wildix_Update_Contacts_in_Phonebook(restclient, phonebook_id, contact_list, first_contact_id);
                }

                foreach (WildixContact contact in Insert_List)
                {
                    Wildix_Insert_Contacts_in_Phonebook(restclient, phonebook_id, contact_list);
                }
            }
            catch
            {
                return "FAIL";
            }
            return "OK";
        }

        // Builds a request about a contact
        public static RestRequest Wildix_Contact_Request_Build(RestRequest request, WildixContact contact)
        {
            request.AddHeader("header", "application/x-www-form-urlencoded");

            request.AddParameter("data[name]", contact.Name);
            request.AddParameter("data[email]", contact.Email);
            request.AddParameter("data[extension]", contact.Extension);
            request.AddParameter("data[phone]", contact.Phone);
            request.AddParameter("data[office]", contact.Office);
            request.AddParameter("data[home]", contact.Home);
            request.AddParameter("data[home_mobile]", contact.Home_mobile);
            request.AddParameter("data[fax]", contact.Fax);
            request.AddParameter("data[mobile]", contact.Mobile);
            request.AddParameter("data[organization]", contact.Organization);
            request.AddParameter("data[type]", contact.Type);
            request.AddParameter("data[address]", contact.Address);
            request.AddParameter("data[vat_id]", contact.vat_id);
            request.AddParameter("data[document_type]", contact.Document_type);
            request.AddParameter("data[document_id]", contact.Document_id);
            request.AddParameter("data[country]", contact.Country);
            request.AddParameter("data[date_birth]", contact.Date_birth);
            request.AddParameter("data[born_in]", contact.Born_in);
            request.AddParameter("data[postal_code]", contact.Postal_code);
            request.AddParameter("data[city]", contact.City);
            request.AddParameter("data[province]", contact.Province);
            request.AddParameter("data[license_plate]", contact.License_plate);
            request.AddParameter("data[document_released]", contact.Document_released);
            request.AddParameter("data[document_from]", contact.Document_from);
            request.AddParameter("data[phonebook_id]", contact.Phonebook_id);
            request.AddParameter("data[note]", contact.Note);
            request.AddParameter("data[pin]", contact.Pin);
            request.AddParameter("data[credit]", contact.Credit);
            request.AddParameter("data[picture]", contact.Picture);
            request.AddParameter("data[name_prefix]", contact.Name_prefix);
            request.AddParameter("data[shortcut]", contact.Shortcut);

            return request;
        }
    }

    public class WildixContact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Extension { get; set; }
        public string Phone { get; set; }
        public string Office { get; set; }
        public string Home { get; set; }
        public string Home_mobile { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Organization { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string vat_id { get; set; }
        public string Document_type { get; set; }
        public string Document_id { get; set; }
        public string Country { get; set; }
        public string Date_birth { get; set; }
        public string Born_in { get; set; }
        public string Postal_code { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string License_plate { get; set; }
        public string Document_released { get; set; }
        public string Document_from { get; set; }
        public string Phonebook_id { get; set; }
        public string Note { get; set; }
        public int Pin { get; set; }
        public int Credit { get; set; }
        public string Picture { get; set; }
        public string Name_prefix { get; set; }
        public string Shortcut { get; set; }

    }

}
