import * as React from 'react';
import SearchForm from 'src/components/searchform';
import './App.css';

import logo from './logo.svg';

class App extends React.Component {
  public render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h1 className="App-title">Demo React Application</h1>
        </header>
        <p className="App-intro">
          Enter the keywords and URL to find into the form below and hit Search button.
        </p>
        <SearchForm />
      </div>
    );
  }
}

export default App;
