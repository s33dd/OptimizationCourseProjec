using System;

namespace OptimizationCourseProject {
    public class PointException : Exception {
        public PointException() { }
        public PointException(string message)
        : base(message) { }
        public PointException(string message, Exception inner)
            : base(message, inner) { }
    }
}
