import React, { useState } from 'react';
import { View, TextInput, Switch } from 'react-native';

const App = () => {
  const [inputValues, setInputValues] = useState({ int1: '500', int2: '550', bool: 0 });

  const handleIntChange = (name, value) => {
    setInputValues(prevState => ({ ...prevState, [name]: parseInt(value) }));
  };

  const toggleSwitch = () => {
    setInputValues(prevState => ({ ...prevState, bool: prevState.bool === 0 ? 1 : 0 }));
    sendDataToArduino();
  };

  const sendDataToArduino = () => {
    fetch('http://192.168.1.144:8000', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: inputValues.bool.toString() + "," + inputValues.int1.toString() + "," + inputValues.int2.toString(),
    })
    .then(response => response.json())
    .then(data => console.log(data))
    .catch((error) => {
      console.error('Error:', error);
    });
  };

  return (
    <View>
      <TextInput 
        keyboardType='numeric'
        onChangeText={(value) => handleIntChange('int1', value)}
        placeholder="LightLevel"
        value={inputValues.int1}
      />
      <TextInput 
        keyboardType='numeric'
        onChangeText={(value) => handleIntChange('int2', value)}
        placeholder="SoundLevel"
        value={inputValues.int2}
      />
      <Switch
        trackColor={{ false: "#767577", true: "#81b0ff" }}
        thumbColor={inputValues.bool === 1 ? "#f5dd4b" : "#f4f3f4"}
        ios_backgroundColor="#3e3e3e"
        onValueChange={toggleSwitch}
        value={inputValues.bool === 0}
      />
    </View>
  );
};

export default App;