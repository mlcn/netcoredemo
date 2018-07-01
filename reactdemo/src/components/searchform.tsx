import * as React from 'react'
import IRequest from 'src/models/request'

interface ISearchFormState {
    keywords: string;
    keywordsError: string;
    result: string;
    resultError: string;
    url: string;
    urlError: string;
}

const initialState: ISearchFormState = {
    keywords: "",
    keywordsError: "",
    result: "",
    resultError: "",
    url: "",
    urlError: ""
}

export default class SearchForm extends React.Component<any, ISearchFormState> {
    constructor(props : any) {
        super(props);
        this.state = initialState
    }

    public render(){
        return(
        <div>
            <p>Keywords</p> <p className="keywordsError">{this.state!.keywordsError}</p>
            <input type="text" className="keywords" value={this.state!.keywords} onChange={this.handleKeywordsChanged} />
            <p>URL to find</p> <p className="urlError">{this.state!.urlError}</p>
            <input type="text" className="url" value={this.state!.url} onChange={this.handleURLChanged}/>
            <br/><br/>
            <button type="submit" name="searchBtn" onClick={this.doSearch}>Search</button>
            <p>{this.state!.result}</p> <p className="resultsError">{this.state!.resultError}</p>
        </div>)
    }

    private doSearch = () => {

        this.setState({keywordsError: "", urlError:"", resultError:"", result:""});

        if (!this.state!.keywords)
        {
            this.setState({keywordsError: "No Keywords specified"});
            return;
        }

        if (!this.validateUrl(this.state!.url))
        {
            this.setState({urlError:"Invalid URL"});
            return;
        }

        let keywordsArray = this.state!.keywords.split(',').map(x => x.trim());
        if (keywordsArray.length === 1)
        {
            keywordsArray = this.state!.keywords.split(' ').map(x => x.trim());
        }
        
        const request: IRequest = {
            Keywords: keywordsArray,
            NumberOfResults: 100,
            UrlToFind: this.state!.url,
        };

        this.fetchSearchResponse(request);
    }

    private validateUrl (value:string)
    {
        const expression = /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi
        const regexp = new RegExp(expression);
        return regexp.test(value);
    } 

    private fetchSearchResponse = (request:IRequest) => {
        fetch("http://localhost:23963/api/search", {
            body: JSON.stringify(request),  
            headers: {
                'Accept': 'application/json',
                'Access-Control-Allow-Origin':'*',
                'Content-Type': 'application/json',
            },
            method: 'POST', 
            mode: 'cors'
        })
        .then(response => response.json())
        .then(json => this.handleSearchResponse(json))
        .catch(error => this.setState({resultError:error.toString()}));
    }

    private handleSearchResponse = (response: string[]) =>{
        this.setState({result: response.join(',')})
    }

    private handleKeywordsChanged = (event:any) => {
        this.setState({keywords: event.target.value})
    }

    private handleURLChanged = (event:any) => {
        this.setState({url:event.target.value})
    }
}