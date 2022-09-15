using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    //[Route("api/[controller]")] will match to controller name
    public class CitiesController : ControllerBase
        {
        //    [HttpGet]
        //    public JsonResult GetCities() 
        //    {
        //        return new JsonResult(CitiesDataStore.Current.Cities);
        //            //new List<object>
        //            //{
        //            //    new{id =1, Name = "New York City" },
        //            //    new{id =2, Name = "Antwerp"}
        //            //});
        //    }


        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
            {
                return Ok(CitiesDataStore.Current.Cities);
       
            }
            //[HttpGet("{id}")]
            //public JsonResult GetCity(int id)
            //{
            //    //return new JsonResult(CitiesDataStore.Current.Cities.Where(c => c.Id == id));
            //    return new JsonResult(CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
            //}

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
