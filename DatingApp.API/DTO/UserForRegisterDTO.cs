using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace DatingApp.API.DTO
{
    //[JsonObject(MemberSerialization.OptIn)]
    public class UserForRegisterDTO
    {
        [Required]
        //[JsonProperty("username")]
        public string Username {get; set;}

        [Required]
        //[JsonProperty("password")]
        [StringLength(8, MinimumLength=4, ErrorMessage="You must specify the password between 4 and 8 characters.")]
        public string Password {get; set;}
    }
}