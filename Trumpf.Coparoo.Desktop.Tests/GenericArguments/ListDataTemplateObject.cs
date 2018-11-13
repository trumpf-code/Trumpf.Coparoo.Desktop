namespace CoparooInterfaceTest
{
    public interface IListDataTemplateObject : IDataTemplateObjectBase
    {
    }

    public class ListDataTemplateObject : DataTemplateObjectBase<FrameworkElement>, IListDataTemplateObject
    {
    }
}
