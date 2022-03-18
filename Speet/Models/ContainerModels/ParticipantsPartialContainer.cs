using Microsoft.AspNetCore.Http;

namespace Speet.Models.ContainerModels
{
    public class ParticipantsPartialContainer
    {
        public SportGroup SportGroup { get; set; }
        public HttpContext HttpContext { get; set; }
    }
}
