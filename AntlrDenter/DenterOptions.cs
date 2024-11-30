/**
 * AntlrDenter, as on https://github.com/yshavit/antlr-denter.
 * 
 * The MIT License (MIT)
 *
 * Copyright (c) 2014 Yuval Shavit:
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
 */

namespace AntlrDenter
{
    public interface IDenterOptions {
        /**
         * Don't do any special handling for EOFs; they'll just be passed through normally. That is, we won't unwind indents
         * or add an extra NL.
         *
         * This is useful when the lexer will be used to parse rules that are within a line, such as expressions. One use
         * case for that might be unit tests that want to exercise these sort of "line fragments".
         */
        void IgnoreEof();
    }
}