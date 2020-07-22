public struct JointWithRotation
{
        public Joint Joint;
        public float Angle;

        public JointWithRotation(Joint joint)
        {
                Joint = joint;
                Angle = 0f;
        }

        public void Deconstruct(out Joint joint, out float angle)
        {
                joint = Joint;
                angle = Angle;
        }
}