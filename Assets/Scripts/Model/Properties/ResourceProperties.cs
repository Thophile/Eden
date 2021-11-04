namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class ResourceProperties
    {
        public ResourceProperties(float protein, float sugar, float fat, float fiber, float minerals)
        {
            this.protein = protein;
            this.sugar = sugar;
            this.fat = fat;
            this.fiber = fiber;
            this.minerals = minerals;
        }
        public ResourceProperties() { }

        public float protein = 0f;
        public float sugar = 0f;
        public float fat = 0f;
        public float fiber = 0f;
        public float minerals = 0f;

        public static ResourceProperties operator *(ResourceProperties a, float b) => new ResourceProperties(a.protein * b, a.sugar * b, a.fat * b, a.fiber * b, a.minerals * b);
        public static ResourceProperties operator *(float b, ResourceProperties a) => new ResourceProperties(a.protein * b, a.sugar * b, a.fat * b, a.fiber * b, a.minerals * b);

        public static ResourceProperties operator *(ResourceProperties a, int b) => new ResourceProperties(a.protein * b, a.sugar * b, a.fat * b, a.fiber * b, a.minerals * b);
        public static ResourceProperties operator *(int b, ResourceProperties a) => new ResourceProperties(a.protein * b, a.sugar * b, a.fat * b, a.fiber * b, a.minerals * b);

        public override string ToString() => $"{protein} Protein, {sugar} Sugar, {fat} Fat, {fiber} Fiber, {minerals} Minerals";

        public static ResourceProperties operator +(ResourceProperties a, ResourceProperties b) => new ResourceProperties(a.protein + b.protein, a.sugar + b.sugar, a.fat + b.fat, a.fiber + b.fiber, a.minerals + b.minerals);

    }
}