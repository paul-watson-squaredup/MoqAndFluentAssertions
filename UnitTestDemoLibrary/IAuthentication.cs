namespace UnitTestDemoLibrary
{
    public interface IAuthentication
    {
        void AuthorizeKey(Key key);

        void UpdateKey(Key key);
    }
}