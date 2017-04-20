using UnityEngine;

public class DirectionUtil
{
    public const int UP = 0;
    public const int RIGHT_UP = 1;
    public const int RIGHT = 2;
    public const int RIGHT_DOWN = 3;
    public const int DOWN = 4;
    public const int LEFT_DOWN = 5;
    public const int LEFT = 6;
    public const int LEFT_UP = 7;

    public static Vector2 UP_VEC = new Vector2(0, 1);
    public static Vector2 RIGHT_UP_VEC = new Vector2(1, 1);
    public static Vector2 RIGHT_VEC = new Vector2(1, 0);
    public static Vector2 RIGHT_DOWN_VEC = new Vector2(1, -1);
    public static Vector2 DOWN_VEC = new Vector2(0, -1);
    public static Vector2 LEFT_DOWN_VEC = new Vector2(-1, -1);
    public static Vector2 LEFT_VEC = new Vector2(-1, 0);
    public static Vector2 LEFT_UP_VEC = new Vector2(-1, 1);

    public static int Vec3ToAngle(Vector3 dir)
    {
        dir.z = 0;
        if (dir.y > 0)
        {
            return 360 - (int)Vector3.Angle(Vector3.right, dir);
        }
        else
        {
            return (int)Vector3.Angle(Vector3.right, dir);
        }
    }

    public static bool IsFlip(int direction)
    {
        if (direction >= LEFT_DOWN)
            return true;
        return false;
    }

    public static int GetAnimationDirection(int direction)
    {
        if (direction == LEFT_DOWN)
            return RIGHT_DOWN;
        if (direction == LEFT)
            return RIGHT;
        if (direction == LEFT_UP)
            return RIGHT_UP;
        return direction;
    }

    public static Vector2 GetDirVec(int direction)
    {
        Vector2 result;
        switch (direction)
        {
            case UP:
                result = UP_VEC;
                break;
            case RIGHT_UP:
                result = RIGHT_UP_VEC;
                break;
            case RIGHT:
                result = RIGHT_VEC;
                break;
            case RIGHT_DOWN:
                result = RIGHT_DOWN_VEC;
                break;
            case DOWN:
                result = DOWN_VEC;
                break;
            case LEFT_DOWN:
                result = LEFT_DOWN_VEC;
                break;
            case LEFT:
                result = LEFT_VEC;
                break;
            case LEFT_UP:
                result = LEFT_UP_VEC;
                break;
            default:result = UP_VEC;
                break;
        }
        result.Normalize();
        return result;
    }

    public static int GetDirection(float angle)
    {
        if (angle >= -22.5 && angle < 22.5)
            return RIGHT;
        if (angle >= 22.5 && angle < 77.5)
            return RIGHT_UP;
        if (angle >= 77.5 && angle < 122.5)
            return UP;
        if (angle >= 122.5 && angle < 167.5)
            return LEFT_UP;
        if (angle >= -67.5 && angle < -22.5)
            return RIGHT_DOWN;
        if (angle >= -122.5 && angle < -67.5)
            return DOWN;
        if (angle >= -167.5 && angle < -122.5)
            return LEFT_DOWN;
        return LEFT;
    }
}
