using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Helpers;

namespace SZGUIFeleves.Models
{
    public class Car : Rectangle
    {
        public const double MAX_WHEEL_TURN_ANGLE = 40;
        public const double BRAKE_SPEED_PERC = 0.7f;
        public const double FRICTION_PERC = 0.9f;
        public const double ROTATION_SPEED = 100.0f;
        public const double SPEED = 100.0f;

        public Vec2d Velocity;
        public Vec2d Acceleration;
        public double RotationRad
        {
            get
            {
                return MathHelper.ConvertToRadians(Rotation);
            }
        }
        public double WheelAngle { get; set; }
        public double WheelAngleRad
        {
            get
            {
                return MathHelper.ConvertToRadians(WheelAngle);
            }
        }

        public Vec2d FrontMiddle
        {
            get
            {
                Vec2d middle = GetMiddle();
                middle += new Vec2d(Size.x / 2 * Math.Cos(RotationRad), Size.y / 2 * Math.Sin(RotationRad));
                return middle;
            }
        }

        public Vec2d FrontMiddleOut
        {
            get
            {
                Vec2d middle = GetMiddle();
                middle += new Vec2d(Size.x * Math.Cos(RotationRad+WheelAngleRad), Size.y * Math.Sin(RotationRad+ WheelAngleRad));
                return middle;
            }
        }

        //TODO Car pos (world space) to screen pos(screen space) conversion
        public Car(Vec2d pos, Vec2d size) : base(pos, size)
        {
            Acceleration = new Vec2d();
            Velocity = new Vec2d();
            Position = pos;
            Rotation = 0;
            WheelAngle = 0;

            flatf = new Vec2d();
            flatr = new Vec2d();
            fTraction = new Vec2d();
        }

        public void Move(double speed)
        {
            //Acceleration += new Vec2d(Math.Cos(RotationAngleRad + WheelAngleRad) * speed, Math.Sin(RotationAngleRad + WheelAngleRad) * speed);
            //acc += speed;
        }

        public void RotateWheel(double angle)
        {
            WheelAngle += angle;
            if (WheelAngle > MAX_WHEEL_TURN_ANGLE)
                WheelAngle = MAX_WHEEL_TURN_ANGLE;
            else if (WheelAngle < -MAX_WHEEL_TURN_ANGLE)
                WheelAngle = -MAX_WHEEL_TURN_ANGLE;
        }

        public void Brake()
        {
            Velocity *= BRAKE_SPEED_PERC;
        }


        /*
        VEC2	velocity;
	    VEC2	acceleration_wc;
	    double	rot_angle;
	    double	sideslip;
	    double	slipanglefront;
	    double	slipanglerear;
	    VEC2	force;
	    int		rear_slip;
	    int		front_slip;
	    VEC2	resistance;
	    VEC2	acceleration;
	    double	torque;
	    double	angular_acceleration;
	    double	sn, cs;
	    double	yawspeed;
	    double	weight;
	    VEC2	ftraction;
	    VEC2	flatf, flatr;

        */

        public double angularvelocity { get; set; }
        public double rotAngle { get; set; }
        public double sideSlip{ get; set; }
        public double slipAngleFront{ get; set; }
        public double slipAngleRear{ get; set; }
        public double weight{ get; set; }
        public Vec2d flatf { get; set; }
        public Vec2d flatr { get; set; }
        public Vec2d fTraction { get; set; }
        public bool frontSlip = true;
        public bool rearSlip = true;

        public const double CA_R = -5.2f;
        public const double CA_F = -5.0f;
        public const double DRAG = 5.0f;
        public const double RESISTANCE = 30.0f;
        public const double MAX_GRIP = 2.0f;

        public void Update()
        {
            //if (WheelAngle < 0)
            //    WheelAngle += Velocity.Length;
            //else if (WheelAngle > 0)
            //    WheelAngle -= Velocity.Length;

            Velocity = new Vec2d(Math.Cos(RotationRad), Math.Sin(RotationRad));

            double yawSpeed = 10 * angularvelocity;

            if (Velocity.x == 0)
            {
                rotAngle = 0;
                sideSlip = 0;
            }
            else
            {
                rotAngle = Math.Atan2(yawSpeed, Velocity.x);
                sideSlip = Math.Atan2(Velocity.y, Velocity.x);
            }

            slipAngleFront = sideSlip + rotAngle - WheelAngle;
            slipAngleRear = sideSlip - rotAngle;

            //carmass * grav
            weight = 1500 * 9.8 * 0.5;

            flatf.x = 0;
            flatf.y = CA_F * slipAngleFront;
            flatf.y = Math.Min(MAX_GRIP, flatf.y);
            flatf.y = Math.Max(-MAX_GRIP, flatf.y);
            if (frontSlip)
                flatf.y *= 0.5;

            flatr.x = 0;
            flatr.y = CA_R * slipAngleRear;
            flatr.y = Math.Min(MAX_GRIP, flatr.y);
            flatr.y = Math.Max(-MAX_GRIP, flatr.y);
            if (rearSlip)
                flatr.y *= 0.5;

            //https://github.com/spacejack/carphysics2d/blob/master/marco/Cardemo.c







            Position += Velocity;

            Velocity *= FRICTION_PERC;

            Acceleration.Zero();
            //acc = 0;
        }
    }
}
