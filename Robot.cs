using System.Numerics;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

public abstract class Target
{
    public bool IsAlive { get; set; } = true;
    public abstract void TakeDamage();
}

public class Animal : Target
{
    public override void TakeDamage()
    {
        IsAlive = false; 
        Console.WriteLine("An animal has been killed.");
    }
}

public class Human : Target
{
    public override void TakeDamage()
    {
        IsAlive = false;
        Console.WriteLine("A human has been killed.");
    }
}

public class Superhero : Target
{
    public override void TakeDamage()
    {
        IsAlive = false;
        Console.WriteLine("A superhero has been killed.");
    }
}


public class Planet
{
    public List<Target> Targets { get; set; } = new List<Target>();

    public bool ContainsLife => Targets.Any(t => t.IsAlive);
}


public enum Intensity
{
    Stun,
    Kill
}

public class GiantKillerRobot
{
    public Intensity EyeLaserIntensity { get; set; }
    public List<Target> Target { get; set; }
    public Target CurrentTarget { get; private set; }
    public bool Active { get; set; } = true;

    public void Initialize()
    {
        
    }

    public void AcquireNextTarget()
    {
        CurrentTarget = Target.FirstOrDefault(t => t.IsAlive);
    }

    public void FireLaserAt(Target target)
    {
        if (EyeLaserIntensity == Intensity.Kill)
        {
            target.TakeDamage();
        }
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        var robot = new GiantKillerRobot();
        robot.Initialize();

        robot.EyeLaserIntensity = Intensity.Kill;
        robot.Target = new List<Target>
        {
            new Animal(),
            new Human(),
            new Superhero()
        };

        var earth = new Planet
        {
            Targets = robot.Target
        };

        while (robot.Active && earth.ContainsLife)
        {
            if (robot.CurrentTarget == null || !robot.CurrentTarget.IsAlive)
            {
                robot.AcquireNextTarget();
            }

            if (robot.CurrentTarget != null && robot.CurrentTarget.IsAlive)
            {
                robot.FireLaserAt(robot.CurrentTarget);
            }
        }
    }
}
