/*
AssetValidator 
Copyright (c) 2018 Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace JCMG.AssetValidator.Editor.Meta
{
    public class SortedEnum<T>
    {
        private readonly IList<T> _allTypeValues;

        public SortedEnum()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("Generic type T must be an enum.");

            _allTypeValues = (T[])Enum.GetValues(typeof(T));
        }

        public IList<T> GetAllGreaterThan(T type)
        {
            var typeVal = Convert.ToInt32(type);
            return _allTypeValues.Where(x => typeVal < Convert.ToInt32(x)).ToList();
        }

        public IList<T> GetAllGreaterThanOrEqualTo(T type)
        {
            var typeVal = Convert.ToInt32(type);
            return _allTypeValues.Where(x => typeVal <= Convert.ToInt32(x)).ToList();
        }

        public IList<T> GetAllLesserThan(T type)
        {
            var typeVal = Convert.ToInt32(type);
            return _allTypeValues.Where(x => typeVal > Convert.ToInt32(x)).ToList();
        }

        public IList<T> GetAllLesserThanOrEqualTo(T type)
        {
            var typeVal = Convert.ToInt32(type);
            return _allTypeValues.Where(x => typeVal >= Convert.ToInt32(x)).ToList();
        }

        public IList<T> GetAllValues()
        {
            return new List<T>(_allTypeValues);
        }
    }
}
