using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

public static class GAController
{
    private static Timer timer;
    private static bool timeElapsed;

    public static List<Child> GeneticAlgorithm(int[,] grid, int size, Vector2Int start, Vector2Int finish, out bool isTimeout)
    {
        List<Child> bestOfGeneration = new List<Child>();
        List<Child> population = RandomPopulation(size);
        bool found = false;
        while (!found && !timeElapsed)
        {
            TimerStart();
            population = SortForFitness(grid,size, population, start, finish);
            bestOfGeneration.Add(population[0]);
            //population[0].fitnessScore <= 0
            if (bestOfGeneration.Count > 100 || population[0].dist <= 0) 
            {
                found = true;
                break;
            }

            float fitnessSum = 0;
            int i = 0;
            for (; i < population.Count * 0.1f; i++)
            {
                fitnessSum += 1f / population[i].fitnessScore;
            }

            for (int j = i; j < population.Count*0.9f; j++)
            {
                Child parent1 = SelectParent(fitnessSum, population);
                Child parent2 = SelectParent(fitnessSum, population);
                Child newChild = Reproduce(parent1, parent2);
                population[j] = newChild;
            }

        }
        if (found)
        {
            isTimeout = false;
            return bestOfGeneration;
        }
        else if(timeElapsed)
        {
            isTimeout = true;
            return null;
        }
        isTimeout = false;
        return null;
    }

    private static Child Reproduce(Child parent1, Child parent2)
    {
        Child child = new Child();
        child.path = CrossOver(parent1, parent2);
        if(UnityEngine.Random.value < 0.1f)
        {
            Mutation(child);
        }
        return child;
    }

    private static void Mutation(Child child)
    {

            int mutationPoint = UnityEngine.Random.Range(0, child.path.Count);
            child.path[mutationPoint] = UnityEngine.Random.Range(1,5);
    }

    private static List<int> CrossOver(Child parent1, Child parent2)
    {
        List<int> path = new List<int>();
        int divisionPoint = UnityEngine.Random.Range(0, parent1.path.Count);
        for (int i = 0; i < parent1.path.Count; i++)
        {
            if (i < divisionPoint)
                path.Add(parent1.path[i]);
            else
                path.Add(parent2.path[i]);
        }
        return path;
    }

    private static Child SelectParent(float fitnessSum, List<Child> population)
    {
        int randomIndex = UnityEngine.Random.Range(0, (int)(population.Count * 0.1f));
        return population[randomIndex];

        //float randomWeight = UnityEngine.Random.Range(0, fitnessSum);
        //int i = 0;
        //for (; i < population.Count * 0.3f; i++)
        //{
        //    if (randomWeight < (1f / population[i].fitnessScore))
        //    {
        //        return population[i];
        //    }
        //    randomWeight -= 1f / population[i].fitnessScore;
        //}
        //return population[i - 1];
    }

    private static List<Child> SortForFitness(int[,] grid, int size, List<Child> population, Vector2Int start,Vector2Int finish)
    {
        Child child;
        bool hitWall, foundFinish;
        int j, traveledUntilHit, hitWallCount, traveled, noLoop;
        Vector2Int childPos;
        Vector2Int moveOffset = Vector2Int.zero;
        Vector2Int moveOffsetPrev = Vector2Int.zero;
        Vector2Int prevPos;

        float recordDist = 100000f;
        float dist;

        for (int i = 0; i < population.Count; i++)
        {
            child = population[i];
            foundFinish = hitWall = false;
            noLoop = traveled = traveledUntilHit = hitWallCount = j = 0;
            childPos = start;
            while ( j < child.path.Count )
            {
                prevPos = childPos;
                moveOffset = DetermineOffset(child.path[j], moveOffset);
                childPos += moveOffset;

                if (j >= 2)
                {
                    moveOffsetPrev = DetermineOffset(child.path[j-2], moveOffsetPrev);
                    prevPos += moveOffsetPrev;
                    if (childPos != prevPos && moveOffset != moveOffsetPrev)
                    {
                        noLoop++;
                    }

                }

                if (childPos.x < 0 || childPos.x > size - 1 || childPos.y < 0 || childPos.y > size - 1 ||grid[childPos.x,childPos.y] == 1 )
                {
                    hitWall = true;
                    hitWallCount++;
                    //childPos -= moveOffset;
                    
                }
                else
                {
                    traveled++;
                }
                //else if (grid[childPos.x, childPos.y] == 2)
                //{
                //    foundFinish = true;
                //    break;
                //}
                if(!hitWall)
                    traveledUntilHit++;
                j++;
            }


            //if (foundFinish)
            //    child.fitnessScore = 0;
            //else

            dist = Vector2Int.Distance(childPos, finish);
            child.dist = dist;
            child.fitnessScore = 1f/dist;
            //child.fitnessScore *= traveledUntilHit;
            child.fitnessScore = Mathf.Pow(child.fitnessScore, 4);
            //child.fitnessScore *= traveledUntilHit*4;
            //child.fitnessScore = traveledUntilHit; //(Math.Abs(finish.x - childPos.x) + Math.Abs(finish.y - childPos.y)) + (size*size - pathNumUntilHit);
            if (hitWall) child.fitnessScore *= 0.1f;
            //1f/dist*dist
            //if (dist < recordDist)
            //{
            //    recordDist = dist;
            //    child.fitnessScore *= 2f;
            //}


        }
        return population.OrderByDescending(x => x.fitnessScore).ToList();
    }

    public static Vector2Int DetermineOffset(int direction, Vector2Int moveOffset)
    {
        if (direction == (int)Directions.L)
        {
            moveOffset.x = -1;
            moveOffset.y = 0;
        }
        else if (direction == (int)Directions.U)
        {
            moveOffset.x = 0;
            moveOffset.y = 1;
        }
        else if (direction == (int)Directions.R)
        {
            moveOffset.x = 1;
            moveOffset.y = 0;
        }
        else if (direction == (int)Directions.D)
        {
            moveOffset.x = 0;
            moveOffset.y = -1;
        }

        return moveOffset;
    }

    private static void TimerStart()
    {
        timer = new Timer(60000);
        timer.Elapsed += Timer_Elapsed;
        timer.Enabled = true;
    }

    private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        timeElapsed = true;
    }

    private static List<Child> RandomPopulation(int size)
    {
        List<Child> population = new List<Child>();
        Child child;
        for (int i = 0; i < size*size; i++)
        {
            child = new Child(RandomPath(size));
            population.Add(child);
        }
        return population;
    }

    private static List<int> RandomPath(int size)
    {
        List<int> path = new List<int>();
        for (int i = 0; i < size*size/4; i++)
        {
            path.Add(UnityEngine.Random.Range(1, 5));
        }
        return path;
    }
}

public class Child
{
    public List<int> path;
    public float fitnessScore;
    public float dist;

    public Child(List<int> path)
    {
        this.path = path;
    }
    public Child()
    {

    }
}
