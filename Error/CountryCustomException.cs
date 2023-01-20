namespace CountryAPI.Error
{
    public class CountryCustomException : Exception
    {
        public string AdditionalInfo { get; set; }
        public string Type { get; set; }
        public string Detail { get; set; }
        public string Title { get; set; }
        public string Instance { get; set; }
        public CountryCustomException(string instance)
        {
            Type = "country-custom-exception";
            Detail = "There was an unexpected error while fetching the product.";
            Title = "Custom Country Exception";
            AdditionalInfo = "Maybe you can try again in a bit?";
            Instance = instance;
        }
    }
}
