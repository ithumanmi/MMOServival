using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //int[] digits = { 1, 2, 9 };
        //int[] result = PlusOne(digits);
        //Debug.Log(string.Join(", ", result)); // Output: 1, 3, 0


        //int[] nums = { -1, 0, 1, 2, -1, -4 };
        //var result = ThreeSum(nums);

        //foreach (var triplet in result)
        //{
        //    Debug.Log(string.Join(", ", triplet));
        //}

        //int[] nums = { -1, 2, 1, -4 };
        //int target = 1;
        //int result = ThreeSumClosest(nums, target);
        //Debug.Log(result);

        int[] nums = { 1, 0, -1, 0, -2, 2 };
        int target = 0;
        var result = FourSum(nums, target);
        foreach (var quadruplet in result)
        {
            Debug.Log($"[{string.Join(", ", quadruplet)}]");
        }
    }
    public int[] PlusOne(int[] digits)
    {
        int n = digits.Length;
        int carry = 1;

        for(int i = n - 1; i >= 0; i--)
        {
            digits[i] += carry;
            if (digits[i] < 10)
            {
                carry = 0;
                break;
            }
            else
            {
                digits[i] = 0;
            }
        }
        if(carry == 1)
        {
            int[] result = new int[n + 1];
            result[0] = 1;
            for(int i = 0; i < n; i++)
            {
                result[i + 1] = digits[i];  
            }
            return result;
        }
        return digits;
    }
    public IList<IList<int>> ThreeSum(int[] nums)
    {
        var result = new List<IList<int>>();
        Array.Sort(nums);

        for(int i = 0; i < nums.Length - 2; i++)
        {
            if (i > 0 && nums[i] == nums[i - 1]) continue;
            int left = i + 1;
            int right = nums.Length - 1;

            while(left < right)
            {
                int sum = nums[i] + nums[left] + nums[right];
                if(sum == 0)
                {
                    result.Add(new List<int> { nums[i], nums[left], nums[right] });

                    while (left < right && nums[left] == nums[left + 1]) left++;
                    while (left < right && nums[right] == nums[right - 1]) right--;

                    left++;
                    right--;
                }
                else if(sum < 0)
                {
                    left++;
                }
                else
                {
                    right--;
                }

            }
        }
        return result;
    }
    public int ThreeSumClosest(int[] nums, int target)
    {
        Array.Sort(nums);
        int closestSum = int.MaxValue - Math.Abs(target);
        for (int i = 0; i < nums.Length - 2; i++)
        {
            int left = i + 1;
            int right = nums.Length - 1;
            while (left < right)
            {
                int currentSum = nums[i] + nums[left] + nums[right];
                if (Math.Abs(target - currentSum) < Math.Abs(target - closestSum))
                {
                    closestSum = currentSum;
                }

                if (currentSum < target)
                {
                    left++;
                }
                else if (currentSum > target)
                {
                    right--;
                }
                else
                {
                    return currentSum;
                }
            }
        }

        return closestSum;
    }
    public IList<IList<int>> FourSum(int[] nums, int target)
    {
        var result = new List<IList<int>>();
        Array.Sort(nums);
        for(int i = 0; i < nums.Length - 3; i++)
        {
            if (i > 0 && nums[i] == nums[i - 1]) continue;

            for(int j = i +  1; j < nums.Length - 2; j++)
            {
                if(j > i + 1 && nums[j] == nums[j - 1]) continue;

                int left = j + 1;
                int right = nums.Length - 1;

                while(left < right)
                {
                    long sum = (long)nums[i] + nums[j] + nums[left] + nums[right];
                    
                    if(sum == target)
                    {
                        result.Add(new List<int>{
                            nums[i], nums[j], nums[left], nums[right]
                        });

                        while(left < right && nums[left] == nums[left + 1]) left++;
                        while(left < right && nums[right] == nums[right - 1]) right--;

                        left++;
                        right--;    
                    }
                    else if(sum < target)
                    {
                        left++;
                    }
                    else
                    {
                        right--;    
                    }

                }
            }
        }

        return result;
    }
}
