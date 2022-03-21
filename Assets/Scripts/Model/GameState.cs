using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class GameState
    {
        public float gameTime;
        public int antNb;
        public int? seed;
        public List<MonoBehaviourData> monobehaviours;
        public ResourceProperties resources;
        public PheromonesMap pheromonesMap;

        public GameState() {
            gameTime = 0f;
            antNb = 200;
            seed = null;
            monobehaviours = new List<MonoBehaviourData>();
            resources = new ResourceProperties();
            pheromonesMap = new PheromonesMap();
        }
    }
}
