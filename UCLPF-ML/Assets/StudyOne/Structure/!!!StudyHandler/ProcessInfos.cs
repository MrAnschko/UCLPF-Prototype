using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProcessInfos
{
    [Serializable]
    public enum InteractionMethod{
        View,
        Point,
        Circle,
        METHOD_COUNT
    }

    [Serializable]
    public enum BeaconClass
    {
        Few,
        Many,
        CLASS_COUNT
    }

    public static int COMBINATION_POSSIBILITIES
    {
        get
        {
            int retval = 1;
            for (int factorial = (int)InteractionMethod.METHOD_COUNT; factorial > 1; factorial--)
            {
                retval *= factorial;
            }
            return retval;
        }
    }

    public static float CYCLE_TIME = 4f;
    public static ModeSettings UCLPF_MODE;
    public static string UserID = "DEFAULT";
    private static bool RNG_Set = false;

    public static int CurrentStep = 0;
    public static InteractionMethod CurrentMethod;
    public static int PathCount = 0; // The point at which we are in the paths
    public static Path currentPath; // The Path that is active

    public static float timeAtStartPath;
    public static float timeAtStartStep;
    public static float PathTime
    {
        get => Time.time-timeAtStartPath;
    }

    public static float StepTime
    {
        get => Time.time - timeAtStartPath;
    }

    [Serializable]
    public enum Path
    {
        One, 
        Two,
        Three,
        PATH_COUNT
    }

    public static int CombinationToInt((InteractionMethod,Path) Combination)
    {
        return (int)Combination.Item1 *(int) InteractionMethod.METHOD_COUNT+(int)Combination.Item2;
    }

    public static (InteractionMethod,Path) IntToCombination(int i)
    {
        int path_index = i % (int)InteractionMethod.METHOD_COUNT;
        int method_index = i / (int)InteractionMethod.METHOD_COUNT;

        return ((InteractionMethod)path_index, (Path)method_index);

    }

    // Assumes that each method can only occur once.
    public static int MethodOrderToInt(List<InteractionMethod> methods)
    {
        int return_value = 0;

        for (int index= 0; index < (int)InteractionMethod.METHOD_COUNT-1; index++)
        {
            // For each index 
            InteractionMethod method = methods[index];
            int method_nr = (int)method; // Find the number associated
            for (int i = 0; i < index; i++)
            {
                method_nr = method_nr < (int)methods[i] ? method_nr : method_nr - 1; // Reduce the number by one for each choice that was not possible
            }
            return_value += method_nr;
            return_value *= (int)InteractionMethod.METHOD_COUNT - index-1;

        }

        return return_value;
    }

    // Assumes an order of all Methods
    public static List<InteractionMethod> IntToMethodOrder(int i)
    {
        List<InteractionMethod> ret_list = new();

        int b = COMBINATION_POSSIBILITIES;

        for (int method = 0; method<(int)InteractionMethod.METHOD_COUNT; method++)
        {
            ret_list.Add((InteractionMethod) method);
        }

        
        for (int index = 0; index < (int)InteractionMethod.METHOD_COUNT - 1; index++)
        {
            b = b / ((int)InteractionMethod.METHOD_COUNT - index);
            int method_nr = i / b; // Calculate the index of the

            // Select element from remaining and move Chosen element to front. 
            InteractionMethod method = ret_list[index + method_nr];
            ret_list.RemoveAt(index + method_nr);
            ret_list.Insert(index,method);
            
            // Calculate remainder
            i = i % b;
            //
            
        }
        return ret_list;
    }


    public static string SetNewID()
    {
        if (!RNG_Set)
        {

            UnityEngine.Random.InitState(System.DateTime.Now.Second); 
            RNG_Set = true;
        }
        int intID = UnityEngine.Random.Range((int)0, (int)1E6);
        UserID = intID.ToString("D6");
        return UserID;
    }
}
