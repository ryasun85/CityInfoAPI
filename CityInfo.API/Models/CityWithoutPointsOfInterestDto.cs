namespace CityInfo.API.Models
{
    /// <summary>
    /// A DTO for city without points of interest 
/   // </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        /// The id of the city
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// the name of the city
        /// </summary>
        public string Name { get; set; } = String.Empty;
        /// <summary>
        /// The description of the city
        /// </summary>
        public string? Description { get; set; }
    }
}
