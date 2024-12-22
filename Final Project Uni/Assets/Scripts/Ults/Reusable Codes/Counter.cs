using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter
{
    public string name;
    public float time;
    public bool IsRunning => time > 0;
    // Start is called before the first frame update
    public Counter(string name, float time)
    {
        this.name = name;
        this.time = time;

    }
    public void Execute()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Exit();
            }
        }
    }
    public void Exit()
    {
        //executesomething
    }
    public void Cancel()
    {
        time = -1;
    }

}
