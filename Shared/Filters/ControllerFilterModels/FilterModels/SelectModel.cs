namespace CoreLibrary.Shared.Filters.ControllerFilterModels.FilterModels
{
    public class SelectModel
    {
        public required PropertySelector[] Properties { get; set; }
    }

    public class PropertySelector
    {
        public required string Name { get; set; }
        public bool IsObject { get; set; } = false;
    }

}
