namespace CoreLibrary.Filters.ControllerFilterModels
{
    public class OrderByModel
    {
        public required string PropertyName { get; set; }
        public bool Descending { get; set; } = false;
    }
}
