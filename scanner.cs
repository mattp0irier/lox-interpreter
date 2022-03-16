using System;
using System.Collections;

namespace LoxInterpreter {

    class Scanner {
        private String source;
        private List<Token> tokens = new List<Token>();

        Scanner(String source) {
            this.source = source;
        }
    }

}