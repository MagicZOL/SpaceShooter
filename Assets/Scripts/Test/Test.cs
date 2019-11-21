public class Engine
{
    public string name;
}

interface IPart
{
    void TestFunc();
}
public class EngineA : Engine, IPart
{
    public EngineA()
    {
        name = "A Engine";
    }

    public void StartEngineA()
    {
        //TODO : 
    }
    public int GetEngineHP()
    {
        return 10;
    }

    public void TestFunc()
    {

    }
}

public class EngineB :Engine
{
    public EngineB()
    {
        name = "B Engine";
    }
    public void StartEngineB()
    {
        //TODO : 
    }
}

public class TireA
{
    string name;

    public TireA()
    {
        name = "A Tire";
    }
}

public class TireB : IPart
{
    string name;

    public TireB()
    {
        name = "B Tire";
    }

    public void TestFunc()
    {

    }
}
public class Car
{
    //엔진과 관련된 속성에 접근하고, 서로 관련이 있는 것일경우(엔진A,엔진B) : 상속
    //서로 관련이 없는 경우(엔진,기름,핸들) : 인터페이스
    //단순히 엔진을 읽고 싶을 경우 : Object
    string name;
    public Engine engine { get; }
    public TireA tire { get; }

    IPart part;

    public Car()
    {
        name = "Car";
        engine = new EngineA();
        engine = new EngineB();
        tire = new TireA();

        part = new EngineA();
        part = new TireB();
    }

    public void Run()
    {
        //TODO:달려
    }
}
public class Test 
{
    private Car car;

    public Car Car
    {
        get
        {
            return car;
        }
    }
    public Test()
    {
        car = new Car();
    }

    void SomeMethod()
    {
        if(car != null)
        {
            if (car.engine != null)
            {
                //car.engine.StartEngineA();
            }
        }
    }

    //int hp = car.engine.GetEngineHP();
   // int? hp2 = car?.engine?.GetEngineHP();
}
