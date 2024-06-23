using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

public abstract class Ball
{
    protected static Random Random = new Random();

    public float Radius { get; set; }
    public PointF Position { get; set; }
    public Color Color { get; set; }
    public PointF Direction { get; set; }

    public Ball()
    {
        Radius = Random.Next(10, 20);
        Position = new PointF(Random.Next(0, 800), Random.Next(0, 600));
        Color = Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256));
        Direction = new PointF((float)(Random.NextDouble() - 0.5), (float)(Random.NextDouble() - 0.5));
    }

    public abstract void OnCollision(Ball other);
}

public class RegularBall : Ball
{
    public override void OnCollision(Ball other)
    {
        if (other is RegularBall)
        {
            if (this.Radius >= other.Radius)
            {
                this.Radius += other.Radius;
                this.Color = CombineColors(this.Color, other.Color, this.Radius, other.Radius);
                other.Radius = 0;
            }
        }
        else if (other is MonsterBall)
        {
            other.Radius += this.Radius;
            this.Radius = 0;
        }
        else if (other is RepellentBall)
        {
            other.Color = this.Color;
            this.Direction = new PointF(-this.Direction.X, -this.Direction.Y);
        }
    }

    private Color CombineColors(Color c1, Color c2, float r1, float r2)
    {
        int r = (int)((c1.R * r1 + c2.R * r2) / (r1 + r2));
        int g = (int)((c1.G * r1 + c2.G * r2) / (r1 + r2));
        int b = (int)((c1.B * r1 + c2.B * r2) / (r1 + r2));
        return Color.FromArgb(r, g, b);
    }
}

public class MonsterBall : Ball
{
    public MonsterBall()
    {
        Direction = new PointF(0, 0);
    }

    public override void OnCollision(Ball other)
    {
        if (other is RegularBall || other is RepellentBall)
        {
            this.Radius += other.Radius;
            other.Radius = 0;
        }
    }
}

public class RepellentBall : Ball
{
    public override void OnCollision(Ball other)
    {
        if (other is RepellentBall)
        {
            var tempColor = this.Color;
            this.Color = other.Color;
            other.Color = tempColor;
        }
        else if (other is MonsterBall)
        {
            this.Radius /= 2;
        }
    }
}

public class Canvas
{
    public int Width { get; }
    public int Height { get; }
    public List<Ball> Balls { get; }

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        Balls = new List<Ball>();
    }

    public void AddBall(Ball ball)
    {
        Balls.Add(ball);
    }

    public void Update()
    {
        for (int i = 0; i < Balls.Count; i++)
        {
            var ball = Balls[i];

            if (ball.Radius == 0)
            {
                Balls.RemoveAt(i);
                i--;
                continue;
            }

            ball.Position = new PointF(ball.Position.X + ball.Direction.X, ball.Position.Y + ball.Direction.Y);

            if (ball.Position.X - ball.Radius < 0 || ball.Position.X + ball.Radius > Width)
            {
                ball.Direction = new PointF(-ball.Direction.X, ball.Direction.Y);
            }
            if (ball.Position.Y - ball.Radius < 0 || ball.Position.Y + ball.Radius > Height)
            {
                ball.Direction = new PointF(ball.Direction.X, -ball.Direction.Y);
            }

            for (int j = i + 1; j < Balls.Count; j++)
            {
                var otherBall = Balls[j];
                if (IsCollision(ball, otherBall))
                {
                    ball.OnCollision(otherBall);
                    otherBall.OnCollision(ball);
                }
            }
        }
    }

    private bool IsCollision(Ball ball1, Ball ball2)
    {
        float dx = ball1.Position.X - ball2.Position.X;
        float dy = ball1.Position.Y - ball2.Position.Y;
        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

        return distance < ball1.Radius + ball2.Radius;
    }

    public bool IsSimulationFinished()
    {
        return !Balls.OfType<RegularBall>().Any();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Canvas canvas = new Canvas(800, 600);

        int numRegularBalls = 10;
        int numMonsterBalls = 2;
        int numRepellentBalls = 3;

        for (int i = 0; i < numRegularBalls; i++)
        {
            canvas.AddBall(new RegularBall());
        }

        for (int i = 0; i < numMonsterBalls; i++)
        {
            canvas.AddBall(new MonsterBall());
        }

        for (int i = 0; i < numRepellentBalls; i++)
        {
            canvas.AddBall(new RepellentBall());
        }

        while (!canvas.IsSimulationFinished())
        {
            canvas.Update();
            //Thread.Sleep(100);
            Console.WriteLine("Update");
        }

        Console.WriteLine("Simulation finished. All regular balls have been absorbed.");
    }
}

