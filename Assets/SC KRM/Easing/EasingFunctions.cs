/*
 * Created by C.J. Kimberlin
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2019
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * 
 * TERMS OF USE - EASING EQUATIONS
 * Open source under the BSD License.
 * Copyright (c)2001 Robert Penner
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE 
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *
 * ============= Description =============
 *
 * Below is an example of how to use the easing functions in the file. There is a getting function that will return the function
 * from an enum. This is useful since the enum can be exposed in the editor and then the function queried during Start().
 * 
 * EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;
 * EasingFunction.EasingFunc func = GetEasingFunction(ease;
 * 
 * double value = func(0, 10, 0.67f);
 * 
 * EasingFunction.EaseingFunc derivativeFunc = GetEasingFunctionDerivative(ease);
 * 
 * double derivativeValue = derivativeFunc(0, 10, 0.67f);
 */

using System;

namespace SCKRM.Easing
{
    [WikiDescription("Easing Function 관련 메소드가 들어있는 클래스 입니다")]
    public static class EasingFunction
    {
        public enum Ease
        {
            Linear = 0,
            Spring,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            EaseInBounce,
            EaseOutBounce,
            EaseInOutBounce,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
            EaseInElastic,
            EaseOutElastic,
            EaseInOutElastic,
        }

        private const double NATURAL_LOG_OF_2 = 0.693147181f;

        //
        // Easing functions
        //

        public static double Linear(double start, double end, double value) => start.Lerp(end, value);

        public static double Spring(double start, double end, double value)
        {
            value = value.Clamp01();
            value = (value * Math.PI * (0.2 + 2.5 * value * value * value) * (1 - value).Pow(2.2) + value) * (1 + (1.2 * (1 - value)));
            return start + (end - start) * value;
        }

        public static double EaseInQuad(double start, double end, double value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static double EaseOutQuad(double start, double end, double value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static double EaseInOutQuad(double start, double end, double value)
        {
            value /= .5;
            end -= start;
            if (value < 1)
                return end * 0.5 * value * value + start;
            value--;
            return -end * 0.5 * (value * (value - 2) - 1) + start;
        }

        public static double EaseInCubic(double start, double end, double value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static double EaseOutCubic(double start, double end, double value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static double EaseInOutCubic(double start, double end, double value)
        {
            value /= .5;
            end -= start;
            if (value < 1)
                return end * 0.5 * value * value * value + start;
            value -= 2;
            return end * 0.5 * (value * value * value + 2) + start;
        }

        public static double EaseInQuart(double start, double end, double value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static double EaseOutQuart(double start, double end, double value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static double EaseInOutQuart(double start, double end, double value)
        {
            value /= .5;
            end -= start;
            if (value < 1)
                return end * 0.5 * value * value * value * value + start;
            value -= 2;
            return -end * 0.5 * (value * value * value * value - 2) + start;
        }

        public static double EaseInQuint(double start, double end, double value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static double EaseOutQuint(double start, double end, double value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static double EaseInOutQuint(double start, double end, double value)
        {
            value /= .5;
            end -= start;
            if (value < 1)
                return end * 0.5 * value * value * value * value * value + start;
            value -= 2;
            return end * 0.5 * (value * value * value * value * value + 2) + start;
        }

        public static double EaseInSine(double start, double end, double value)
        {
            end -= start;
            return -end * (value * (Math.PI * 0.5)).Cos() + end + start;
        }

        public static double EaseOutSine(double start, double end, double value)
        {
            end -= start;
            return end * (value * (Math.PI * 0.5)).Sin() + start;
        }

        public static double EaseInOutSine(double start, double end, double value)
        {
            end -= start;
            return -end * 0.5 * ((Math.PI * value).Cos() - 1) + start;
        }

        public static double EaseInExpo(double start, double end, double value)
        {
            end -= start;
            return end * 2d.Pow(10 * (value - 1)) + start;
        }

        public static double EaseOutExpo(double start, double end, double value)
        {
            end -= start;
            return end * (-(2d.Pow(-10 * value)) + 1) + start;
        }

        public static double EaseInOutExpo(double start, double end, double value)
        {
            value /= .5;
            end -= start;
            if (value < 1)
                return end * 0.5 * 2d.Pow(10 * (value - 1)) + start;
            value--;
            return end * 0.5 * (-(2d.Pow(- 10 * value)) + 2) + start;
        }

        public static double EaseInCirc(double start, double end, double value)
        {
            end -= start;
            return -end * ((1 - value * value).Sqrt() - 1) + start;
        }

        public static double EaseOutCirc(double start, double end, double value)
        {
            value--;
            end -= start;
            return end * (1 - value * value).Sqrt() + start;
        }

        public static double EaseInOutCirc(double start, double end, double value)
        {
            value /= .5;
            end -= start;
            if (value < 1)
                return -end * 0.5 * ((1 - value * value).Sqrt() - 1) + start;
            value -= 2;
            return end * 0.5 * ((1 - value * value).Sqrt() + 1) + start;
        }

        public static double EaseInBounce(double start, double end, double value)
        {
            end -= start;
            double d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        public static double EaseOutBounce(double start, double end, double value)
        {
            value /= 1;
            end -= start;
            if (value < (1 / 2.75))
            {
                return end * (7.5625 * value * value) + start;
            }
            else if (value < (2 / 2.75))
            {
                value -= (1.5 / 2.75);
                return end * (7.5625 * (value) * value + .75) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25 / 2.75);
                return end * (7.5625 * (value) * value + .9375) + start;
            }
            else
            {
                value -= (2.625 / 2.75);
                return end * (7.5625 * (value) * value + .984375) + start;
            }
        }

        public static double EaseInOutBounce(double start, double end, double value)
        {
            end -= start;
            double d = 1;
            if (value < d * 0.5)
                return EaseInBounce(0, end, value * 2) * 0.5 + start;
            else
                return EaseOutBounce(0, end, value * 2 - d) * 0.5 + end * 0.5 + start;
        }

        public static double EaseInBack(double start, double end, double value)
        {
            end -= start;
            value /= 1;
            double s = 1.70158;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static double EaseOutBack(double start, double end, double value)
        {
            double s = 1.70158;
            end -= start;
            value = (value) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static double EaseInOutBack(double start, double end, double value)
        {
            double s = 1.70158;
            end -= start;
            value /= .5;
            if ((value) < 1)
            {
                s *= (1.525);
                return end * 0.5 * (value * value * (((s) + 1) * value - s)) + start;
            }
            value -= 2;
            s *= (1.525);
            return end * 0.5 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static double EaseInElastic(double start, double end, double value)
        {
            end -= start;

            double d = 1;
            double p = d * .3;
            double s;
            double a = 0;

            if (value == 0)
                return start;

            if ((value /= d) == 1)
                return start + end;

            if (a == 0 || a < end.Abs())
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Math.PI) * (end / a).Asin();
            }

            return -(a * 2d.Pow(10 * (value -= 1)) * ((value * d - s) * (2 * Math.PI) / p).Sin()) + start;
        }

        public static double EaseOutElastic(double start, double end, double value)
        {
            end -= start;

            double d = 1;
            double p = d * .3;
            double s;
            double a = 0;

            if (value == 0)
                return start;

            if ((value /= d) == 1)
                return start + end;

            if (a == 0 || a < end.Abs())
            {
                a = end;
                s = p * 0.25;
            }
            else
            {
                s = p / (2 * Math.PI) * (end / a).Asin();
            }

            return (a * 2d.Pow(-10 * value) * ((value * d - s) * (2 * Math.PI) / p).Sin() + end + start);
        }

        public static double EaseInOutElastic(double start, double end, double value)
        {
            end -= start;

            double d = 1f;
            double p = d * .3f;
            double s;
            double a = 0;

            if (value == 0)
                return start;

            if ((value /= d * 0.5d) == 2)
                return start + end;

            if (a == 0f || a < end.Abs())
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Math.PI) * (end / a).Asin();
            }

            if (value < 1)
                return -0.5 * (a * 2d.Pow(10 * (value -= 1)) * ((value * d - s) * (2 * Math.PI) / p).Sin()) + start;
            return a * 2d.Pow(-10 * (value -= 1)) * ((value * d - s) * (2 * Math.PI) / p).Sin() * 0.5 + end + start;
        }

        //
        // These are derived functions that the motor can use to get the speed at a specific time.
        //
        // The easing functions all work with a normalized time (0 to 1) and the returned value here
        // reflects that. Values returned here should be divided by the actual time.
        //
        // TODO: These functions have not had the testing they deserve. If there is odd behavior around
        //       dash speeds then this would be the first place I'd look.

        public static double LinearD(double start, double end, double value) => end - start;

        public static double EaseInQuadD(double start, double end, double value) => 2 * (end - start) * value;

        public static double EaseOutQuadD(double start, double end, double value)
        {
            end -= start;
            return -end * value - end * (value - 2);
        }

        public static double EaseInOutQuadD(double start, double end, double value)
        {
            value /= .5;
            end -= start;

            if (value < 1)
                return end * value;

            value--;

            return end * (1 - value);
        }

        public static double EaseInCubicD(double start, double end, double value) => 3d * (end - start) * value * value;

        public static double EaseOutCubicD(double start, double end, double value)
        {
            value--;
            end -= start;
            return 3 * end * value * value;
        }

        public static double EaseInOutCubicD(double start, double end, double value)
        {
            value /= .5;
            end -= start;

            if (value < 1)
            {
                return (3d / 2d) * end * value * value;
            }

            value -= 2;

            return (3d / 2d) * end * value * value;
        }

        public static double EaseInQuartD(double start, double end, double value)
        {
            return 4 * (end - start) * value * value * value;
        }

        public static double EaseOutQuartD(double start, double end, double value)
        {
            value--;
            end -= start;
            return -4 * end * value * value * value;
        }

        public static double EaseInOutQuartD(double start, double end, double value)
        {
            value /= .5;
            end -= start;

            if (value < 1)
            {
                return 2 * end * value * value * value;
            }

            value -= 2;

            return -2 * end * value * value * value;
        }

        public static double EaseInQuintD(double start, double end, double value)
        {
            return 5 * (end - start) * value * value * value * value;
        }

        public static double EaseOutQuintD(double start, double end, double value)
        {
            value--;
            end -= start;
            return 5 * end * value * value * value * value;
        }

        public static double EaseInOutQuintD(double start, double end, double value)
        {
            value /= .5;
            end -= start;

            if (value < 1)
            {
                return (5d / 2d) * end * value * value * value * value;
            }

            value -= 2;

            return (5d / 2d) * end * value * value * value * value;
        }

        public static double EaseInSineD(double start, double end, double value) => (end - start) * 0.5 * Math.PI * (0.5 * Math.PI * value).Sin();

        public static double EaseOutSineD(double start, double end, double value)
        {
            end -= start;
            return (Math.PI * 0.5) * end * (value * (Math.PI * 0.5)).Cos();
        }

        public static double EaseInOutSineD(double start, double end, double value)
        {
            end -= start;
            return end * 0.5 * Math.PI * (Math.PI * value).Sin();
        }
        public static double EaseInExpoD(double start, double end, double value) => (10 * NATURAL_LOG_OF_2 * (end - start) * 2d.Pow(10d * (value - 1)));

        public static double EaseOutExpoD(double start, double end, double value)
        {
            end -= start;
            return 5 * NATURAL_LOG_OF_2 * end * 2d.Pow(1 - 10 * value);
        }

        public static double EaseInOutExpoD(double start, double end, double value)
        {
            value /= .5;
            end -= start;

            if (value < 1)
                return 5 * NATURAL_LOG_OF_2 * end * 2d.Pow(10d * (value - 1));

            value--;

            return (5 * NATURAL_LOG_OF_2 * end) / (2d.Pow(10d * value));
        }

        public static double EaseInCircD(double start, double end, double value) => ((end - start) * value) / (1d - value * value).Sqrt();

        public static double EaseOutCircD(double start, double end, double value)
        {
            value--;
            end -= start;
            return (-end * value) / (1 - value * value).Sqrt();
        }

        public static double EaseInOutCircD(double start, double end, double value)
        {
            value /= .5;
            end -= start;

            if (value < 1)
            {
                return (end * value) / (2 * (1 - value * value).Sqrt());
            }

            value -= 2;

            return (-end * value) / (2 * (1 - value * value).Sqrt());
        }

        public static double EaseInBounceD(double start, double end, double value)
        {
            end -= start;
            const double d = 1;

            return EaseOutBounceD(0, end, d - value);
        }

        public static double EaseOutBounceD(double start, double end, double value)
        {
            value /= 1;
            end -= start;

            if (value < (1 / 2.75))
                return 2 * end * 7.5625 * value;
            else if (value < (2 / 2.75))
            {
                value -= (1.5 / 2.75);
                return 2 * end * 7.5625 * value;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25 / 2.75);
                return 2 * end * 7.5625 * value;
            }
            else
            {
                value -= (2.625 / 2.75);
                return 2 * end * 7.5625 * value;
            }
        }

        public static double EaseInOutBounceD(double start, double end, double value)
        {
            end -= start;
            const double d = 1;

            if (value < d * 0.5)
                return EaseInBounceD(0, end, value * 2) * 0.5;
            else
                return EaseOutBounceD(0, end, value * 2 - d) * 0.5;
        }

        public static double EaseInBackD(double start, double end, double value)
        {
            const double s = 1.70158;
            return 3d * (s + 1) * (end - start) * value * value - 2 * s * (end - start) * value;
        }

        public static double EaseOutBackD(double start, double end, double value)
        {
            const double s = 1.70158;
            end -= start;
            value = (value) - 1;

            return end * ((s + 1) * value * value + 2 * value * ((s + 1) * value + s));
        }

        public static double EaseInOutBackD(double start, double end, double value)
        {
            double s = 1.70158;
            end -= start;
            value /= .5;

            if ((value) < 1)
            {
                s *= (1.525);
                return 0.5 * end * (s + 1) * value * value + end * value * ((s + 1) * value - s);
            }

            value -= 2;
            s *= (1.525);
            return 0.5 * end * ((s + 1) * value * value + 2 * value * ((s + 1f) * value + s));
        }

        public static double EaseInElasticD(double start, double end, double value) => EaseOutElasticD(start, end, 1d - value);

        public static double EaseOutElasticD(double start, double end, double value)
        {
            end -= start;

            double d = 1;
            double p = d * .3;
            double s;
            double a = 0;

            if (a == 0d || a < end.Abs())
            {
                a = end;
                s = p * 0.25;
            }
            else
                s = p / (2 * Math.PI) * (end / a).Asin();

            return (a * Math.PI * d * 2d.Pow(1 - 10 * value) *
                ((2 * Math.PI * (d * value - s)) / p).Cos()) / p - 5 * NATURAL_LOG_OF_2 * a *
                2d.Pow(1 - 10 * value) * ((2 * Math.PI * (d * value - s)) / p).Sin();
        }

        public static double EaseInOutElasticD(double start, double end, double value)
        {
            end -= start;

            double d = 1;
            double p = d * .3;
            double s;
            double a = 0;

            if (a == 0f || a < end.Abs())
            {
                a = end;
                s = p / 4;
            }
            else
                s = p / (2 * Math.PI) * (end / a).Asin();

            if (value < 1)
            {
                value -= 1;

                return -5 * NATURAL_LOG_OF_2 * a * 2d.Pow(10 * value) * (2 * Math.PI * (d * value - 2d) / p).Sin() -
                    a * Math.PI * d * 2d.Pow(10d * value) * (2 * Math.PI * (d * value - s) / p).Cos() / p;
            }

            value -= 1;

            return a * Math.PI * d * (2 * Math.PI * (d * value - s) / p).Cos() / (p * 2d.Pow(10 * value)) -
                5 * NATURAL_LOG_OF_2 * a * (2 * Math.PI * (d * value - s) / p).Sin() / (2d.Pow(10 * value));
        }

        public static double SpringD(double start, double end, double value)
        {
            value = value.Clamp01();
            end -= start;

            // Damn... Thanks http://www.derivative-calculator.net/
            // TODO: And it's a little bit wrong
            return end * (6 * (1 - value) / 5 + 1) * (-2.2 * (1 - value).Pow(1.2) *
                (Math.PI * value * (2.5 * value * value * value + 0.2)).Sin() + (1 - value).Pow(2.2) *
                (Math.PI * (2.5 * value * value * value + 0.2) + 7.5 * Math.PI * value * value * value) *
                (Math.PI * value * (2.5 * value * value * value + 0.2)).Cos() + 1) -
                6 * end * ((1 - value).Pow(2.2) * (Math.PI * value * (2.5 * value * value * value + 0.2)).Sin() + value
                / 5);

        }

        /// <summary>
        /// Computed as a function linked to the easingFunction enum
        /// </summary>
        /// <param name="easingFunction">The enum associated with the easing function.</param>
        /// <returns>The easing function</returns>
        public static double EasingCalculate(double start, double end, double value, Ease easingFunction)
        {
            return easingFunction switch
            {
                Ease.EaseInQuad => EaseInQuad(start, end, value),
                Ease.EaseOutQuad => EaseOutQuad(start, end, value),
                Ease.EaseInOutQuad => EaseInOutQuad(start, end, value),
                Ease.EaseInCubic => EaseInCubic(start, end, value),
                Ease.EaseOutCubic => EaseOutCubic(start, end, value),
                Ease.EaseInOutCubic => EaseInOutCubic(start, end, value),
                Ease.EaseInQuart => EaseInQuart(start, end, value),
                Ease.EaseOutQuart => EaseOutQuart(start, end, value),
                Ease.EaseInOutQuart => EaseInOutQuart(start, end, value),
                Ease.EaseInQuint => EaseInQuint(start, end, value),
                Ease.EaseOutQuint => EaseOutQuint(start, end, value),
                Ease.EaseInOutQuint => EaseInOutQuint(start, end, value),
                Ease.EaseInSine => EaseInSine(start, end, value),
                Ease.EaseOutSine => EaseOutSine(start, end, value),
                Ease.EaseInOutSine => EaseInOutSine(start, end, value),
                Ease.EaseInExpo => EaseInExpo(start, end, value),
                Ease.EaseOutExpo => EaseOutExpo(start, end, value),
                Ease.EaseInOutExpo => EaseInOutExpo(start, end, value),
                Ease.EaseInCirc => EaseInCirc(start, end, value),
                Ease.EaseOutCirc => EaseOutCirc(start, end, value),
                Ease.EaseInOutCirc => EaseInOutCirc(start, end, value),
                Ease.Linear => Linear(start, end, value),
                Ease.Spring => Spring(start, end, value),
                Ease.EaseInBounce => EaseInBounce(start, end, value),
                Ease.EaseOutBounce => EaseOutBounce(start, end, value),
                Ease.EaseInOutBounce => EaseInOutBounce(start, end, value),
                Ease.EaseInBack => EaseInBack(start, end, value),
                Ease.EaseOutBack => EaseOutBack(start, end, value),
                Ease.EaseInOutBack => EaseInOutBack(start, end, value),
                Ease.EaseInElastic => EaseInElastic(start, end, value),
                Ease.EaseOutElastic => EaseOutElastic(start, end, value),
                Ease.EaseInOutElastic => EaseInOutElastic(start, end, value),
                _ => 0
            };
        }

        /// <summary>
        /// Gets the derivative function of the appropriate easing function. If you use an easing function for position then this
        /// function can get you the speed at a given time (normalized).
        /// </summary>
        /// <param name="easingFunction"></param>
        /// <returns>The derivative function</returns>
        public static double EasingDerivativeCalculate(double start, double end, double value, Ease easingFunction)
        {
            return easingFunction switch
            {
                Ease.EaseInQuad => EaseInQuadD(start, end, value),
                Ease.EaseOutQuad => EaseOutQuadD(start, end, value),
                Ease.EaseInOutQuad => EaseInOutQuadD(start, end, value),
                Ease.EaseInCubic => EaseInCubicD(start, end, value),
                Ease.EaseOutCubic => EaseOutCubicD(start, end, value),
                Ease.EaseInOutCubic => EaseInOutCubicD(start, end, value),
                Ease.EaseInQuart => EaseInQuartD(start, end, value),
                Ease.EaseOutQuart => EaseOutQuartD(start, end, value),
                Ease.EaseInOutQuart => EaseInOutQuartD(start, end, value),
                Ease.EaseInQuint => EaseInQuintD(start, end, value),
                Ease.EaseOutQuint => EaseOutQuintD(start, end, value),
                Ease.EaseInOutQuint => EaseInOutQuintD(start, end, value),
                Ease.EaseInSine => EaseInSineD(start, end, value),
                Ease.EaseOutSine => EaseOutSineD(start, end, value),
                Ease.EaseInOutSine => EaseInOutSineD(start, end, value),
                Ease.EaseInExpo => EaseInExpoD(start, end, value),
                Ease.EaseOutExpo => EaseOutExpoD(start, end, value),
                Ease.EaseInOutExpo => EaseInOutExpoD(start, end, value),
                Ease.EaseInCirc => EaseInCircD(start, end, value),
                Ease.EaseOutCirc => EaseOutCircD(start, end, value),
                Ease.EaseInOutCirc => EaseInOutCircD(start, end, value),
                Ease.Linear => LinearD(start, end, value),
                Ease.Spring => SpringD(start, end, value),
                Ease.EaseInBounce => EaseInBounceD(start, end, value),
                Ease.EaseOutBounce => EaseOutBounceD(start, end, value),
                Ease.EaseInOutBounce => EaseInOutBounceD(start, end, value),
                Ease.EaseInBack => EaseInBackD(start, end, value),
                Ease.EaseOutBack => EaseOutBackD(start, end, value),
                Ease.EaseInOutBack => EaseInOutBackD(start, end, value),
                Ease.EaseInElastic => EaseInElasticD(start, end, value),
                Ease.EaseOutElastic => EaseOutElasticD(start, end, value),
                Ease.EaseInOutElastic => EaseInOutElasticD(start, end, value),
                _ => 0
            };
        }
    }
}