using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class GameState
    {
        public float gameTime;
        public int antNb;
        public List<object[]> antsInfo;
        public List<object[]> resourceInfo;
        public ResourceProperties resources;
        public PheromonesMap pheromonesMap;

        public GameState() {
            gameTime = 0f;
            antNb = 200;
            antsInfo = new List<object[]>();
            resourceInfo = new List<object[]>();
            resources = new ResourceProperties();
            pheromonesMap = new PheromonesMap();
        }
    }
}
