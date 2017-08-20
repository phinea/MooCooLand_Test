namespace MooCooEngine.Game
{
    public class User : Singleton<User>
    {
        //# TODO: Make a singleton out of it!
        public string UserName { private set; get; }
        public string UserID { private set; get; }
        public Level LastLevel { private set; get; }
    }
}