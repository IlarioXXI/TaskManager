export class User{
    constructor(
        public email : string,
        public id   : string,
        private _token : string,
        public role : string
    ) {}

    get token() {
        return this._token;
    }
}