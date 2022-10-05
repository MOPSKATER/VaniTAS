namespace VaniTAS
{
    [Serializable()]
    internal record InputAction
    {
        public bool Jump, Fire, Ability, Swap;
        public float Vertical, Horizontal, Pitch, Yaw;

        public override string ToString()
        {
            return "Vertical: " + Vertical.Truncate(4) +
                "\nHorizontal: " + Horizontal.Truncate(4) +
                "\nPitch: " + Pitch.Truncate(4) +
                "\nYaw: " + Yaw.Truncate(4) +
                "\nJump: " + Jump +
                "\nFire: " + Fire +
                "\nAbility: " + Ability +
                "\nSwap: " + Swap;
        }
    }
}
