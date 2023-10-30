namespace CleaningRobot.Model
{
    public enum Command
    {
        /// <summary>
        /// Turn Left (TL). Instructs the robot to turn 90 degrees to the left. Consumes 1 unit of battery. 
        /// </summary>
        TL,

        /// <summary>
        /// Turn Right (TR). Instructs the robot to turn 90 degrees to the right. Consumes 1 unit of battery.
        /// </summary>
        TR,

        /// <summary>
        /// Advance (A). Instructs the robot to advance one cell forward into the next cell. Consumes 2 unit of battery.
        /// </summary>
        A,

        /// <summary>
        /// Back (B). Instructs the robot to move back one cell without changing direction. Consumes 3 units of battery. 
        /// </summary>
        B,

        /// <summary>
        /// Clean (C). Instructs the robot to clean the current cell. Consumes 5 units of battery.
        /// </summary>
        C
    }
}
