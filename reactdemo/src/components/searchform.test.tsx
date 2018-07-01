import * as Adapter from 'enzyme-adapter-react-16';
import * as React from 'react';
import * as enzyme from 'enzyme';
import SearchForm from './searchform';

enzyme.configure({adapter: new Adapter()});

it('Validates keywords', () => {
  const searchform = enzyme.shallow(<SearchForm/>);
  const searchButton = searchform.find("button");
  searchButton.simulate('click');
  expect(searchform.find('keywordsError').text()).toEqual('No Keywords specified')
});

it('Validates URL', () => {
    const searchform = enzyme.shallow(<SearchForm/>);
    const searchButton = searchform.find("button");
    const keywordsText = searchform.find("input")[0];
    keywordsText.simulate('change', { target: { value: 'Changed' } });
    searchButton.simulate('click');
    expect(searchform.find("urlError").text()).toEqual('Invalid URL');
  });
