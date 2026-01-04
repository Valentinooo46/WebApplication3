import React from 'react';
import FlightsTable from './components/FlightsTable';
import CitiesList from './components/CitiesList';
import CountriesList from './components/CountriesList';

const App: React.FC = () => {
    return (
        <div>
            <h1>Remote Application</h1>
            <CountriesList />
            <CitiesList />
            <FlightsTable />
        </div>
    );
};

export default App;