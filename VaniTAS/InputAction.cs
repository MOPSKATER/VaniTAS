namespace TAS
{
    internal struct InputAction
    {
        public bool Jump, Fire, Ability, Swap;
        public float Vertical, Horizontal, Pitch, Yaw;

        public override string ToString()
        {
            return "Vertical: " + Vertical +
                "\nHorizontal: " + Horizontal +
                "\nPitch: " + Pitch +
                "\nYaw: " + Yaw +
                "\nJump: " + Jump +
                "\nFire: " + Fire +
                "\nAbility: " + Ability +
                "\nSwap: " + Swap;
        }
    }
}
