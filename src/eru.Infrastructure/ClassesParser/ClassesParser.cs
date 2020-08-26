using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;

namespace eru.Infrastructure.ClassesParser
{
    public class ClassesParser : IClassesParser
    {
        public Task<Class> Parse(string name)
        {
            name = name.Trim();

            if (name[0] >= 48 && name[0] <= 57)
            {
                if (name[1] >= 48 && name[1] <= 57)
                {
                    var number = int.Parse(name.Substring(0, 2));
                    if (number == 10 || number == 11 || number == 12)
                        return Task.FromResult(new Class(number, name.Substring(2)));
                }

                return Task.FromResult(new Class(int.Parse(name.Substring(0, 1)), name.Substring(1)));
            }

            if (name.StartsWith("I"))
            {
                if (name.StartsWith("II"))
                {
                    if (name.StartsWith("III"))
                    {
                        return Task.FromResult(new Class(3, name.Substring(3)));
                    }

                    return Task.FromResult(new Class(2, name.Substring(2)));
                }

                if (name.StartsWith("IV"))
                {
                    return Task.FromResult(new Class(4, name.Substring(2)));
                }

                if (name.StartsWith("IX"))
                {
                    return Task.FromResult(new Class(9, name.Substring(2)));
                }

                return Task.FromResult(new Class(1, name.Substring(1)));
            }

            if (name.StartsWith("V"))
            {
                if (name.StartsWith("VI"))
                {
                    if (name.StartsWith("VII"))
                    {
                        if (name.StartsWith("VIII"))
                        {
                            return Task.FromResult(new Class(8, name.Substring(4)));
                        }

                        return Task.FromResult(new Class(7, name.Substring(3)));
                    }

                    return Task.FromResult(new Class(6, name.Substring(2)));
                }

                return Task.FromResult(new Class(5, name.Substring(1)));
            }

            if (name.StartsWith("X"))
            {
                if (name.StartsWith("XI"))
                {
                    if (name.StartsWith("XII"))
                    {
                        return Task.FromResult(new Class(12, name.Substring(3)));
                    }

                    return Task.FromResult(new Class(11, name.Substring(2)));
                }

                return Task.FromResult(new Class(10, name.Substring(1)));
            }

            throw new Exception("Class must have a year number!");
        }

        public Task<IEnumerable<Class>> Parse(IEnumerable<string> names)
        {
            var classes = names.ToArray().Select(className => Parse(className).Result).ToList();

            return Task.FromResult(classes.AsEnumerable());
        }
    }
}