namespace Units
{
    public struct TempEntityStats
    {
        public TempEntityStats(float maxHP, float moveSpeed, float damage, float range)
        {
            MaxHealth = maxHP;
            MoveSpeed = moveSpeed;
            Damage = damage;
            Range = range;

            CurrentHealth = MaxHealth;
        }

        public float MaxHealth;
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