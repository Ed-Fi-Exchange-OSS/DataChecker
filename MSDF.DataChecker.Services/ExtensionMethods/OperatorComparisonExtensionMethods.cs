// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

//using MSDF.DataChecker.Services.Models;
//using System;

//namespace MSDF.DataChecker.Services.ExtensionMethods
//{
//    public static class OperatorComparisonExtensionMethods
//    {
//        public static bool Evaluate(this RuleBO rule, int result)
//        {
//            return Evaluate(result, rule.EvaluationOperand, rule.ExpectedResult);
//        }
//        public static bool Evaluate(int operand1, string @operator, int operand2)
//        {
//            switch (@operator)
//            {
//                case ">":
//                    return !(operand1 > operand2);
//                case ">=":
//                case "=>":
//                    return !(operand1 >= operand2);
//                case "==":
//                case "=":
//                    return !(operand1 == operand2);
//                case "<":
//                    return !(operand1 < operand2);
//                case "<=":
//                case "=<":
//                    return !(operand1 <= operand2);
//                case "!=":
//                    return !(operand1 != operand2);
//                default:
//                    throw new NotImplementedException($"Operator ({@operator}) has not been implemented.");
//            }
//        }
//    }
//}
