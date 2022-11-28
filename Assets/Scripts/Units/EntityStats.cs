namespace Units
{
    public struct TempEntityStats
    {
        public TempEntityStats(float moveSpeed, float damage, float range, float currentHealth)
        {
            CurrentHealth = currentHealth;
            MoveSpeed = moveSpeed;
            Damage = damage;
            Range = range;
        }

        public float CurrentHealth;
        public float MoveSpeed;
        public float Damage;
        public float Range;

        public bool IsDead => CurrentHealth <= 0;

        public bool DamageEntity(float value)
        {
            CurrentHealth -= value;
            return IsDead;
        }
    }
}