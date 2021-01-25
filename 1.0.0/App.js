import React, { Component } from 'react';
import { createAppContainer } from 'react-navigation';
import { createStackNavigator} from 'react-navigation-stack';
import welcome from './pages/welcome';

const App = createStackNavigator(
  {
    welcome: {
      screen: welcome,
    },
    
  }
  
);
export default createAppContainer(App);