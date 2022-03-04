using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class GameState
    {
        public float gameTime;
        public int antNb;
        public int? seed;
        public object[] cameraInfo;
        public List<object[]> antsInfo;
        public List<object[]> resourceInfo;
        public ResourceProperties resources;
        public PheromonesMap pheromonesMap;
        // To save :
        // seed -> hmap, assets, colony position
        // + reuse for game loading


        public GameState() {
            gameTime = 0f;
            antNb = 200;
            seed = null;
            antsInfo = new List<object[]>();
            resourceInfo = new List<object[]>();
            resources = new ResourceProperties();
            pheromonesMap = new PheromonesMap();
        }
    }
}
