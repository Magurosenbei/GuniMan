using System.Collections.ObjectModel;


namespace Engine
{
    public class GameScreenCollection : KeyedCollection<string, GameScreen>
    {
        // Allow us to get a screen by name like so:
        // Engine.GameScreens["ScreenName"]
        protected override string GetKeyForItem(GameScreen item)
        {
            return item.Name;
        }

        protected override void RemoveItem(int index)
        {
            // Get the screen to be removed
            GameScreen screen = Items[index];

            // If this screen is the current default screen, set the
            // default to the background screen
            if (GameEngine.DefaultScreen == screen)
                GameEngine.DefaultScreen = GameEngine.BackgroundScreen;

            base.RemoveItem(index);
        }
    }

}
