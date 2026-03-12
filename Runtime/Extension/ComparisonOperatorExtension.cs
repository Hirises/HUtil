using System;

using UnityEngine;

namespace HUtil.Runtime.Extension
{
    public static class ComparisonOperatorExtension
    {
        public static bool Compare(this ComparisonOperator op, int value1, int value2){
            switch(op){
                case ComparisonOperator.Equal:
                    return value1 == value2;
                case ComparisonOperator.NotEqual:
                    return value1 != value2;
                case ComparisonOperator.GreaterThan:
                    return value1 > value2;
                case ComparisonOperator.LessThan:
                    return value1 < value2;
                case ComparisonOperator.GreaterThanOrEqual:
                    return value1 >= value2;
                case ComparisonOperator.LessThanOrEqual:
                    return value1 <= value2;
            }
            return false;
        }
        
        public static bool Compare(this ComparisonOperator op, float value1, float value2){
            switch(op){
                case ComparisonOperator.Equal:
                    return Mathf.Approximately(value1, value2);
                case ComparisonOperator.NotEqual:
                    return !Mathf.Approximately(value1, value2);
                case ComparisonOperator.GreaterThan:
                    return value1 > value2;
                case ComparisonOperator.LessThan:
                    return value1 < value2;
                case ComparisonOperator.GreaterThanOrEqual:
                    return value1 >= value2;
                case ComparisonOperator.LessThanOrEqual:
                    return value1 <= value2;
            }
            return false;
        }

        public static bool Compare(this ComparisonOperator op, string value1, string value2){
            switch(op){
                case ComparisonOperator.Equal:
                    return value1 == value2;
                case ComparisonOperator.NotEqual:
                    return value1 != value2;
            }
            return false;
        }

        public static bool Compare(this ComparisonOperator op, bool value1, bool value2){
            switch(op){
                case ComparisonOperator.Equal:
                    return value1 == value2;
                case ComparisonOperator.NotEqual:
                    return value1 != value2;
            }
            return false;
        }
    }
}