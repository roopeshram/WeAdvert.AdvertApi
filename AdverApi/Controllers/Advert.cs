using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdverApi.Services;
using AdvertApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdverApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class Advert : ControllerBase
    {
        // GET: /<controller>/
        private readonly IadvertStorageService _advertstorageservice;
        public Advert(IadvertStorageService advertstorageservice)
        {
            _advertstorageservice = advertstorageservice;

        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(404)]
        [ProducesResponseType(201, Type=typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordid;
            try
            {
                System.IO.File.WriteAllText(@"C:\inetpub\logfiles\log.txt", "Started");
                System.IO.File.WriteAllText(@"C:\inetpub\logfiles\log.txt", model.Title);
                recordid = await _advertstorageservice.Add(model);
            }
            catch(KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception.Message);
            }
            return StatusCode(201, new CreateAdvertResponse { Id = recordid });
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertstorageservice.Confirm(model);
                
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }
            return new OkResult();
        }
    }
}
