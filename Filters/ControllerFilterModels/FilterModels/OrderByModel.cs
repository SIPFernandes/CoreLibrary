namespace CoreLibrary.Filters.ControllerFilterModels.FilterModels
{
    public class OrderByModel
    {
        public required string PropertyName { get; set; }
        public bool Descending { get; set; } = false;
    }
}
