using Assets.Scripts.Model.Data;
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
        public List<AntData> ants;
        public List<object[]> resourceInfo;
        public ResourceProperties resources;
        public PheromonesMap pheromonesMap;

        public GameState() {
            gameTime = 0f;
            antNb = 200;
            seed = null;
            ants = new List<AntData>();
            resourceInfo = new List<object[]>();
            resources = new ResourceProperties();
            pheromonesMap = new PheromonesMap();
        }
    }
}
